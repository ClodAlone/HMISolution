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
using System.Drawing.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Grid;
using System.Collections.Generic;
#endregion

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	[ToolboxItem( false ),
	Syncfusion.Documentation.DocumentationExclude()
	]
	public class MenuGridControlBase: GridControlBase, IBarItemContainerControl, IMessageFilter
	{
		#region PRIVATE_MEMBERS
        private BrushInfo brush = null;
		private bool menuBound = false;
		protected const int IconColumn = 1;
		protected internal int IconColumnWidth = SystemInformation.MenuCheckSize.Width + 9;
		protected const int TextColumn = 2;
		protected internal int TextColumnWidth = 84;
		protected const int ShortcutColumn = 3;
		protected internal int ShortcutColumnWidth = 50;
		protected const int ArrowColumn = 4;
		protected internal int ArrowColumnWidth = SystemInformation.MenuCheckSize.Width + 9;
		protected readonly int MenuColCount = 4;
		protected GridRangeInfo lastHighlight = GridRangeInfo.Empty;
		internal ParentBarItem parentItem;
		protected int mouseDownItem = -1;
		protected internal bool fixedWidth = false;
		protected Point mouseDownPoint = Point.Empty;
		private Timer setCustomizingTimer;
		private MenuGridAccessibleObject asObject = null;
		private bool initialized = false;
		internal MenuGridControlBaseWeakContainer menuGridControlBaseWeakContainer = null;
		private bool m_bIsVistaOS = false;
		#endregion PRIVATE_MEMBERS
		#region INITIALIZATION
		protected override void InitializeMouseControllers()
		{
			Model.Options.ControllerOptions = GridControllerOptions.None;

			this.MouseControllerDispatcher.Add( new GridSelectCellsMouseController( this ) );
			base.InitializeMouseControllers();
		}

		static MenuGridControlBase()
		{
			GridControlBase.UseOldHiddenScrollLogic = true;
		}

		public MenuGridControlBase()
		{
			this.Model = new GridModel( ToolsComp.GetPublicType() );
			this.SetStyle( ControlStyles.Selectable, false );

			this.HScrollBehavior = GridScrollbarMode.Disabled;
			this.VScrollBehavior = GridScrollbarMode.Disabled;

			this.InitializeComponent();
			Application.AddMessageFilter(this);
			menuGridControlBaseWeakContainer = new MenuGridControlBaseWeakContainer( this );
			MenuColors.MenuColorsChanged += new EventHandler( this.menuGridControlBaseWeakContainer.MenuColorsChangedWeakEventHandler );
			Office2003Colors.MenuColorsChanged += new EventHandler( this.menuGridControlBaseWeakContainer.Office2003ColorsChangedWeakEventHandler );

			this.setCustomizingTimer = new Timer();
			this.setCustomizingTimer.Interval = 300;
			this.setCustomizingTimer.Tick += new EventHandler( SetCustomizingItemTick );
			this.initialized = true;
			this.firstPaint = false;

			this.ForcedVistaStyle = false;
		}

		protected virtual internal void JitCode()
		{
			this.ResizeColumns();
			this.OnQueryCellInfo( 0, 0, GridStyleInfo.Empty, -1 );
			this.MenuBound = this.MenuBound;
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			if( this.ParentItem != null )
			{
				this.asObject = new MenuGridAccessibleObject( this.ParentItem, this );
				return asObject;
			}
			else
				return null;
		}
		BarItem IBarItemContainerControl.HitTest( int x, int y )
		{
			if( this.parentItem != null )
			{
				int index = this.HitTestBarItemIndex( this.PointToClient( new Point( x, y ) ) );
				if( index >= 0 )
					return this.parentItem.Items[index];
				else
					return null;
			}
			return null;
		}
		Rectangle IBarItemContainerControl.GetBoundsOf( BarItem item )
		{
			Rectangle bounds = this.GetBoundsOf( item );
			return this.RectangleToScreen( bounds );
		}
		public Rectangle GetBoundsOf( BarItem item )
		{
			if( this.parentItem != null )
			{
				int gridRow = this.parentItem.Items.IndexOf( item ) + 1;
				if( gridRow != 0 )
				{
					return this.RangeInfoToRectangle( GridRangeInfo.Row( gridRow ), GridRangeOptions.None );
				}
			}

			return Rectangle.Empty;
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

			if( this.MenuBound && this.HighlightRange.Top > 0 )
			{
				BarItem selectedItem = this.parentItem.Items[this.HighlightRange.Top - 1];
				if( item == selectedItem )
					states |= AccessibleStates.Focused;
			}

			return states;
		}
		internal void MenuColorsChanged( object sender, EventArgs e )
		{
			this.UpdateMenuColors();
		}

		private void UpdateMenuColors()
		{
			Model.Properties.BackgroundColor = MenuColors.MenuBGColor;

			GridStyleInfo standard = Model.BaseStylesMap["Standard"].StyleInfo;
            brush = new BrushInfo(MenuColors.MenuBGColor);

            standard.Interior = brush;

			Model.ColWidths[IconColumn] = IconColumnWidth;
		}

        private void ClearBrushInfo()
        {
            if (brush != null)
                brush.ClearColorInfo();
        }

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				//UnwireModel();	UnwireModel is called from the base GridControl.Dispose
                MenuColors.MenuColorsChanged -= new EventHandler(this.menuGridControlBaseWeakContainer.MenuColorsChangedWeakEventHandler);
                Office2003Colors.MenuColorsChanged -= new EventHandler(this.menuGridControlBaseWeakContainer.Office2003ColorsChangedWeakEventHandler);
				MenuColors.MenuColorsChanged -= new EventHandler( this.MenuColorsChanged );
				Office2003Colors.MenuColorsChanged -= new EventHandler( this.MenuColorsChanged );
				this.setCustomizingTimer.Tick -= new EventHandler( SetCustomizingItemTick );
				Application.RemoveMessageFilter(this);
			}
			base.Dispose( disposing );
		}

		protected override void OnSystemColorsChanged( EventArgs e )
		{
			base.OnSystemColorsChanged( e );
			MenuColors.SysColorsChanged( false );
			Office2003Colors.SysColorsChanged( false );
			if( this.MenuBound )
				this.Refresh();
		}
		protected override void WireModel()
		{
			base.WireModel();

			if( Model == null )
				return;

			Model.QueryColCount += new GridRowColCountEventHandler( Model_QueryColCount );
			Model.SaveCellInfo += new GridSaveCellInfoEventHandler( ModelSaveCellInfo );
			Model.QueryCellInfo += new GridQueryCellInfoEventHandler( ModelQueryCellInfo );
			Model.QueryRowCount += new GridRowColCountEventHandler( ModelQueryRowCount );
			Model.QueryRowHeight += new GridRowColSizeEventHandler( ModelQueryRowHeight );
			Model.QueryCoveredRange += new GridQueryCoveredRangeEventHandler( ModelQueryCoveredRange );
		}

		protected override void UnwireModel()
		{
			base.UnwireModel();

			if( Model == null )
				return;

			Model.QueryColCount -= new GridRowColCountEventHandler( Model_QueryColCount );
			Model.SaveCellInfo -= new GridSaveCellInfoEventHandler( ModelSaveCellInfo );
			Model.QueryCellInfo -= new GridQueryCellInfoEventHandler( ModelQueryCellInfo );
			Model.QueryRowCount -= new GridRowColCountEventHandler( ModelQueryRowCount );
			Model.QueryRowHeight -= new GridRowColSizeEventHandler( ModelQueryRowHeight );
			Model.QueryCoveredRange -= new GridQueryCoveredRangeEventHandler( ModelQueryCoveredRange );
		}

		protected virtual void InitializeComponent()
		{
			Model.BeginInit();
			SetupGridMenuLike();
			Model.EndInit();
		}

		protected virtual void SetupGridMenuLike()
		{
			Model.CommandStack.Enabled = false;
			Model.Options.ExcelLikeCurrentCell = false;
			Model.Options.ExcelLikeSelectionFrame = false;
			Model.Options.DefaultGridBorderStyle = GridBorderStyle.None;
			Model.Options.ResizeRowsBehavior = GridResizeCellsBehavior.None;
			Model.Options.ResizeColsBehavior = GridResizeCellsBehavior.None;
			Model.Options.AllowSelection = GridSelectionFlags.None;
			Model.Options.AllowDragSelectedCols = false;
			Model.Options.AllowDragSelectedRows = false;
			Model.Options.HorizontalThumbTrack = false;
			Model.Options.VerticalThumbTrack = false;
			Model.Options.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.HideAlways;
			Model.Options.DefaultGridBorderStyle = GridBorderStyle.None;
			Model.Options.EnterKeyBehavior = GridDirectionType.None;
			//Model.ColCount = MenuColCount;
			Model.Data.ColCount = MenuColCount;
			Model.Properties.BackgroundColor = MenuColors.MenuBGColor;

			Model.Cols.DefaultSize = SystemInformation.MenuCheckSize.Width + 9;
			Model.Rows.DefaultSize = SystemInformation.MenuHeight < 22 ? 22 : SystemInformation.MenuHeight;
			Model.HideCols[0] = true;
			Model.HideRows[0] = true;

			GridStyleInfo standard = Model.BaseStylesMap["Standard"].StyleInfo;
			standard.Interior = new BrushInfo( MenuColors.MenuBGColor );
			Font newfont = SystemInformation.MenuFont;
			this.SetStyleInfoFontFromGdipFont( standard, newfont );
			standard.VerticalAlignment = GridVerticalAlignment.Middle;
			standard.CellType = "Static";
			standard.WrapText = false;
			standard.Tag = null; // can be any kind of object
			standard.CellValueType = typeof( string );
			standard.HorizontalAlignment = GridHorizontalAlignment.Left;

			// Instead treat this as a Covered cell (for performance reasons)
			// So that the text column will float into the shortcut column
			//Model.Options.FloatCellsMode = GridFloatCellsMode.OnDemandCalculation;

			Model.Options.FloatCellsMode = GridFloatCellsMode.None;

			// Standard Edit
			Model.CellModels.Add( "TextBox", new TextBoxCellModel( Model ) );

			// Menu Text
			Model.CellModels.Add( "MenuTextCell", new MenuTextCellModel( Model ) );

			// Scroll cell type
			Model.CellModels.Add( "CheckMarkCell", new CheckMarkCellModel( Model ) );

			// Icon cell type
			Model.CellModels.Add( "IconCell", new IconCellModel( Model ) );

			// MenuGlyph cell type
			Model.CellModels.Add( "MenuGlyphCell", new MenuGlyphCellModel( Model ) );

			// Separator cell type
			Model.CellModels.Add( "SeparatorCell", new SeparatorCellModel( Model ) );

			// Shortcut cell type
			Model.CellModels.Add( "ShortcutCell", new ShortcutCellModel( Model ) );

			// Combobox cell type
			Model.CellModels.Add( "ComboBox", new MenuComboBoxCellModel( Model, true ) );

			// Combobox cell type
			Model.CellModels.Add( "ComboBox_ListboxMode", new MenuComboBoxCellModel( Model, false ) );

			// let's force creation of renderers for all registered cellmodels (should speed up first time display)

			foreach( DictionaryEntry entry in Model.CellModels )
				CellRenderers.Add( (string)entry.Key, ( (GridCellModelBase)entry.Value ).CreateRenderer( this ) );

			InitMenuGrid();
		}

		private void SetStyleInfoFontFromGdipFont( GridStyleInfo si, Font newfont )
		{
			si.Font.FontStyle = newfont.Style;
			si.Font.Facename = newfont.Name;
			si.Font.Size = newfont.SizeInPoints;
		}

		protected virtual void InitMenuGrid()
		{
			// Icon Column 
			Model.ColWidths[IconColumn] = IconColumnWidth;

			// MenuGlyph Column
			Model.ColWidths[ArrowColumn] = ArrowColumnWidth;

			// Need some margin on the left; Make sure to add this space to the text
			// size while calculating the preferred size of the columns in ResizeColumns.
			Model.ColStyles[TextColumn].TextMargins.Left = 5;
			// To update certain colors.
			this.UpdateMenuColors();
		}
		protected virtual void CustomizingItemChanged( object sender, EventArgs e )
		{
			FreeObjectCache();
			this.Refresh();
		}
		protected virtual void Property_Changed( object sender, SyncfusionPropertyChangedEventArgs e )
		{
			if( sender == this.parentItem )
			{
				this.RefreshMenuGrid( false );
			}
			// Ignore separator index changes in the ParentBarItem children.
			else if( e.PropertyName != "SeparatorIndices" )
			{
				// Property changed in a child
				if( e.PropertyChangeEffect == PropertyChangeEffect.NeedRepaint
					// Update only the row even thought these properties could use a full refresh (otherwise results in too much flicker
					|| e.PropertyName == "Text"
					|| e.PropertyName == "ImageIndex"
					|| e.PropertyName == "Shortcut"

					|| ( e.PropertyName == "Checked" && Model.ColWidths[IconColumn] > 0 ) )
				{
					int childIndex = this.parentItem.Items.IndexOf( sender );
					if( childIndex != -1 )
					{
						FreeObjectCache();
						this.InvalidateRange( GridRangeInfo.Row( childIndex + 1 ) );
					}
				}
				else
				{
                    bool canRefresh = e.PropertyName == "Visible";
					this.RefreshMenuGrid( canRefresh );
				}
			}
		}
		private void ChildCollection_Changed( object sender, CollectionChangeEventArgs e )
		{
			try
			{
				BeginUpdate( BeginUpdateOptions.Invalidate );

				this.HighlightRange = GridRangeInfo.Empty;
				this.RefreshMenuGrid( true );

				OnChildCollectionChanged( sender, e );
			}
			finally
			{
				EndUpdate();
			}
		}

		protected virtual void OnChildCollectionChanged( object sender, CollectionChangeEventArgs e )
		{
		}

		#endregion INITIALIZATION

		#region PROPERTIES
		public bool Customizing
		{
			get
			{
				if( this.MenuBound && this.parentItem.Manager != null )
					return this.parentItem.Manager.Customizing;
				else
					return false;
			}
		}
		protected virtual bool MenuBound
		{
			get { return this.menuBound; }
			set
			{
				if( this.menuBound != value )
				{
					this.menuBound = value;
					if( this.menuBound )
					{
						// Otherwise the grid stays scrolled if scrolled in the previous popup state.
						this.TopRowIndex = 1;

						//Listen to the parent's and children's events
						this.parentItem.PropertyChanged += new Syncfusion.ComponentModel.SyncfusionPropertyChangedEventHandler( this.Property_Changed );
						this.parentItem.Items.ItemPropertyChanged += new Syncfusion.ComponentModel.SyncfusionPropertyChangedEventHandler( this.Property_Changed );
						this.parentItem.Items.CollectionChanged += new CollectionChangeEventHandler( this.ChildCollection_Changed );
						if( this.parentItem.Manager != null )
						{
							this.parentItem.Manager.CustomizingItemChanged
								+= new EventHandler( this.CustomizingItemChanged );
						}
						this.UpdateMenuColors();
						if( this.asObject != null )
							this.asObject.SetBarItemContainer( this.parentItem );
					}
					else
					{
						this.CurrentCell.Deactivate( true );
						this.HighlightRange = GridRangeInfo.Empty;
						if( this.parentItem != null )
						{
							this.parentItem.PropertyChanged -= new Syncfusion.ComponentModel.SyncfusionPropertyChangedEventHandler( this.Property_Changed );
							if( this.parentItem.Items != null )
							{
								this.parentItem.Items.ItemPropertyChanged -= new Syncfusion.ComponentModel.SyncfusionPropertyChangedEventHandler( this.Property_Changed );
								this.parentItem.Items.CollectionChanged -= new CollectionChangeEventHandler( this.ChildCollection_Changed );
							}
							if( this.parentItem.Manager != null )
							{
								this.parentItem.Manager.CustomizingItemChanged
									-= new EventHandler( this.CustomizingItemChanged );
							}
							this.parentItem = null;
						}
						//Release references to the ParentBarItem
						if( this.asObject != null )
							this.asObject.SetBarItemContainer( null );
                        this.ClearBrushInfo();
					}
					FreeObjectCache();
				}
			}
		}
		protected void FreeObjectCache()
		{
			ViewLayout.Reset();
			Model.ResetVolatileData();
			Model.FloatingCells.DelayFloatCells( GridRangeInfo.Table() );
		}

		public GridRangeInfo HighlightRange
		{
			get { return lastHighlight; }
			set
			{
				if( this.lastHighlight != value )
				{
					FreeObjectCache();

					GridRangeInfo oldHighlight = this.lastHighlight;

					this.lastHighlight = value;

					if( oldHighlight != GridRangeInfo.Empty )
						this.HideHighlight( oldHighlight );

					if( this.lastHighlight != GridRangeInfo.Empty )
						this.ShowHighlight( this.lastHighlight );
				}
			}
		}
		public int SelectedIndex
		{
			get
			{
				if( !this.MenuBound || this.HighlightRange == GridRangeInfo.Empty )
					return -1;
				return this.HighlightRange.Top - 1;
			}
			set
			{
				if( this.MenuBound )
				{
					if( value == -1 )
						this.HighlightRange = GridRangeInfo.Empty;
					else if( value < this.parentItem.Items.Count )
						this.HighlightRange = GridRangeInfo.Row( value + 1 );
				}
			}
		}
		public BarItem SelectedItem
		{
			get
			{
				if( this.SelectedIndex == -1 )
					return null;
				else
					return this.parentItem.Items[this.SelectedIndex];
			}
			set
			{
				if( this.MenuBound )
				{
					if( value == null )
						this.SelectedIndex = -1;
					else
					{
						int index = this.parentItem.Items.IndexOf( value );
						if( index != -1 )
							this.SelectedIndex = index;
					}
				}
			}
		}
		public ParentBarItem ParentItem
		{
			get { return this.parentItem; }
		}
		internal VisualStyle Style
		{
			get
			{
				if( this.parentItem != null )
					return this.parentItem.Style;
				else
					return VisualStyle.OfficeXP;
			}
		}

		/// <summary>
		/// Gets color table for Office2007 visual style.
		/// </summary>
		private Office2007Colors Office2007ColorTable
		{
			get
			{
				Office2007Colors colorTable = Office2007Colors.Default;

				if( parentItem != null && parentItem.barManager != null
					&& parentItem.barManager.commandBarManager != null )
				{
					CommandBarController controller = parentItem.barManager.commandBarManager.GetCommandBarController();

					if( controller != null )
					{
						colorTable = controller.Office2007ColorTable;
					}
				}

				return colorTable;
			}
		}

		/// <summary>
		/// Indicates whether host OS is Vista.
		/// </summary>
		internal bool IsVistaOS
		{
			get
			{
				return m_bIsVistaOS;
			}
		}

		#endregion PROPERTIES

		#region OVERRIDES

		void ModelSaveCellInfo( object sender, GridSaveCellInfoEventArgs e )
		{
			if (this.parentItem != null && e.RowIndex > 0)
			{
				BarItem bi = this.parentItem.Items[e.RowIndex - 1];
				int col = ( bi.Text.Length > 0 ) ? ShortcutColumn : TextColumn;

				if( e.ColIndex == col )
				{
					IRequiresControl item = bi as IRequiresControl;
					
					if (item != null)
					{
                        if (e.Style.CellType == "ComboBox")
                            item.Value = e.Style.CellValue.ToString().Replace("\n", "") as string;
                        else
                            item.Value = e.Style.CellValue as string;
					}
					
					e.Handled = true;
				}
			}
		}
		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );
			if( e.Button == MouseButtons.Left )
			{
				Point mouseDownPt = new Point( e.X, e.Y );
				this.SetSelectionAt( mouseDownPt );
				this.mouseDownPoint = mouseDownPt;

				this.mouseDownItem = this.HitTestBarItemIndex( mouseDownPt );
				if( !this.IsSelectable( this.mouseDownItem + 1 ) )
					this.mouseDownItem = -1;
			}
		}
		protected override void OnLeftColChanging( GridRowColIndexChangingEventArgs e )
		{
			if( e.Value != 1 )
				e.Cancel = true;
		}
		protected virtual void ResetDragging()
		{
			this.mouseDownItem = -1;
		}
		public static bool ShouldDrawVisible( ParentBarItem parentItem, BarItem item, bool customizing )
		{
			if( item is ListBarItem && !customizing )
			{
				if( ( (ListBarItem)item ).ChildCaptions.Count > 0 )
					return false;	// This never gets shown in the menu, only it's expanded children.
			}

			if( item is SeparatorItem || item.Manager == null )
				return item.Visible;

			MainFrameBarManager manager = item.Manager.MainFrameBarManager;
			bool visible = true;
			if( manager != null )
				visible = manager.ShouldDrawVisible( item );
			else
				visible = item.Visible | customizing;

			if( visible && parentItem != null )
			{
				// Also check with the parent
				visible = parentItem.ShouldDrawVisible( item );
			}

			return visible;
		}
		protected int HitTestBarItemIndex( Point pointAt )
		{
			GridRangeInfo range = this.PointToRangeInfo( pointAt, -1 );
			if( !range.IsEmpty )
				return range.Top - 1;
			else
				return -1;
		}
		#endregion OVERRIDES
		#region MENU_OPERATIONS
		private const int DEF_STEP = 1;
		internal const int DEF_BORDERRADIUS = 2;

		Point m_prevPoint = Point.Empty;

		private bool m_bScrollOnMouseMove = false;

		/// <summary>
		/// Gets or Sets, scroll items in menu, when mouse moves over scroll buttons.
		/// </summary>
		public bool ScrollOnMouseMove
		{
			get
			{
				return m_bScrollOnMouseMove;
			}
			set
			{
				if( value != m_bScrollOnMouseMove )
				{
					m_bScrollOnMouseMove = value;
				}
			}
		}

		protected BarItem SetSelectionAt( Point point )
		{
			GridRangeInfo range = ( HighlightRange == GridRangeInfo.Empty ) ? base.PointToRangeInfo( point, -1 ) : HighlightRange;

			int height = this.Model.RowHeights.GetTotal( 0, Model.RowCount );
			bool bNeedScroll = ( height > this.Height );

			//bool bNeedScroll = base.Model.RowCount * base.Model.RowHeights
			// move up, if point is higher then menu top 
			if( point.Y < 0 && range.Top > 1 && bNeedScroll &&
				ScrollOnMouseMove )
			{
				range = range.OffsetRange( -DEF_STEP, 0 );
			}

			// move down, if point is lower then menu top 
			else if( point.Y > this.Height + MenuGridHost.ScrollHeight && bNeedScroll &&
				ScrollOnMouseMove )
			{
				range = range.OffsetRange( DEF_STEP, 0 );
			}
			else
			{
				range = base.PointToRangeInfo( point, -1 );
			}

			if( range.IsEmpty || range.Top == 0 || range.Bottom > this.Model.RowCount )
			{
				this.HighlightRange = GridRangeInfo.Empty;
				return null;
			}

			return SetSelectionAtRow( range.Top );
		}

		protected BarItem SetSelectionAtRow( int row )
		{
			BarItem barItem = this.parentItem.Items[row - 1];

			if( row > 0 && row <= this.Model.RowCount && this.ShouldDrawEnabled( barItem ) )
			{
				// Update highlight range
				if( row <= Model.RowCount && this.IsSelectable( row ) )
				{
					BarItem selectedItem = row - 1 < this.parentItem.Items.Count ?
						this.parentItem.Items[row - 1] : null;
					if( this.Customizing && selectedItem != null )
					{
						if( this.parentItem.Manager.DesignMode )
							this.parentItem.Manager.CustomizingItem = selectedItem;
						else
							this.SetCustomizingItemDelayed( selectedItem );
					}

					this.HighlightRange = GridRangeInfo.Rows( row, row );
				}
				else
					this.HighlightRange = GridRangeInfo.Empty;
			}
			else
			{
				this.HighlightRange = GridRangeInfo.Empty;
			}

			return barItem;
		}

		protected BarItem cachedSelectedItem;
		private void SetCustomizingItemDelayed( BarItem selectedItem )
		{
			try
			{
				this.setCustomizingTimer.Stop();
			}
			catch { }
			this.cachedSelectedItem = selectedItem;
			this.setCustomizingTimer.Start();
		}
		public BarItem CurrentHotTrackItem
		{
			get { return this.cachedSelectedItem; }
		}
		private void SetCustomizingItemTick( object sender, EventArgs e )
		{
			this.cachedSelectedItem = null;
			try
			{
				this.setCustomizingTimer.Stop();
			}
			catch { }
			if( this.parentItem != null )
				this.parentItem.Manager.CustomizingItem = this.cachedSelectedItem;
		}

		public virtual void HidePopup( PopupCloseType popupCloseType )
		{
			this.MenuBound = false;
		}

		public virtual void Show( ParentBarItem parentItem, Point location,
			IPopupParent parentUI, bool setDefaultSelection )
		{
			Show( parentItem, location, parentUI, setDefaultSelection, null );
		}

		internal virtual void Show( ParentBarItem parentItem, Point location,
			IPopupParent parentUI, bool setDefaultSelection, Queue pbiQueue )
		{
            bool imagedBarItem = false;
            if (!parentItem.OverlapCheckBoxImageBounds)
            {
                BarItems barItems = parentItem.Items;
                {
                    foreach (BarItem baritem in barItems)
                    {
                        if ((baritem.Image != null && baritem.Checked) || (baritem.ImageIndex > 0 && baritem.Checked))
                        {
                            imagedBarItem = true;
                        }
                    }
                }
                if (imagedBarItem)
                {
                    IconColumnWidth = 2 * (SystemInformation.MenuCheckSize.Width + 9);
                }
                else
                {
                    IconColumnWidth = SystemInformation.MenuCheckSize.Width + 9;
                }
            }
            else
            {
                IconColumnWidth = SystemInformation.MenuCheckSize.Width + 9;
            }
            BeginUpdate(BeginUpdateOptions.Invalidate);
            try
            {
                this.parentItem = parentItem;
                this.MenuBound = true;  // will do FreeObjectCache

				if( this.Dock == DockStyle.None && location != Point.Empty )
					this.Location = location;

				this.HighlightRange = GridRangeInfo.Empty;
			}
			finally
			{
				EndUpdate();

				if( this.parentItem.Manager != null )
				{
					MainFrameBarManager mainManager = this.parentItem.Manager.MainFrameBarManager;

					if( mainManager != null )
						this.ThemesEnabled = mainManager.ThemesEnabled;
				}

				this.RefreshMenuGrid( false );
			}
		}

		protected override void Refresh( bool fromModel )
		{
			if( !initialized || Model.Initializing )
				return;

			Model.BeginUpdate( BeginUpdateOptions.None );
			Model.FloatingCells.EvaluateFloatingCells( GridRangeInfo.Table() );
			Model.EndUpdate( false );
			base.Refresh( fromModel );
		}

		private Regex m_regex = new Regex( "[&]{2}|[&]" );

		protected string ReplaceCallBack( Match match )
		{
			return match.Value.Remove( 0, 1 );
		}

		protected virtual int GetApproxWidth( Graphics g, BarItem item, string text )
		{
			GridStyleInfo standard = Model.BaseStylesMap["Standard"].StyleInfo;
			Font font = standard.Font.GdipFont;
			ProvideFontInfoEventArgs args = new ProvideFontInfoEventArgs( font );
			item.OnProvideFontInfo( args );
			font = args.Font;

			// used to measure correctly strings with lot's of '&'
			text = m_regex.Replace( text, new MatchEvaluator( ReplaceCallBack ) );

			return (int)g.MeasureString( text, font, Int32.MaxValue ).Width;
		}

		protected virtual int GetExactWidth( Graphics g, int row, int column )
		{
			return Model.CalculatePreferredCellSize( g, row, column, GridQueryBounds.Width ).Width;
		}

		protected virtual bool IsItemVisibleOrExpandable( BarItem item, bool shouldDrawVisible )
		{
			return (shouldDrawVisible ? item.Visible : false);
		}

		protected internal void ResizeColumns()
		{
			// Will call this method to Jit ahead of time.
			if( !this.MenuBound )
				return;

			if( this.fixedWidth )
			{
				Model.ColWidths[IconColumn] = this.IconColumnWidth;
				Model.ColWidths[TextColumn] = this.TextColumnWidth;
				Model.ColWidths[ShortcutColumn] = this.ShortcutColumnWidth;// - 20;// To compensate for the smaller size of the cust dlg commands menu.
				Model.ColWidths[ArrowColumn] = this.ArrowColumnWidth;
			}
			else
			{
				int combinedMaxWidth = -1;
				int shortcutMaxWidth = -1;
				int textMaxWidth = -1;
				BarItem textMaxWidthItem = null, shortcutMaxWidthItem = null, combinedMaxWidthItem = null;
				// Using floating mode, text column width can be kept smaller than the 
				// required text width.
				bool useOptimalTextAndShortcutColumnWidthsLogic = true;

				Graphics g = this.CreateGraphics();
				int count = this.parentItem.Items.Count;
				for( int i = 1; i <= count; i++ )
				{
					BarItem item = this.parentItem.Items[i - 1] as BarItem;
					bool hasText = ( item.Text.Length > 0 );
					bool shouldDrawVisible = ShouldDrawVisible( this.parentItem, item, this.Customizing );
					if( !this.IsItemVisibleOrExpandable( item, shouldDrawVisible ) )
						continue;

					// Text column text
					int textWidth = this.GetApproxWidth( g, item, item.Text );
					textWidth += 5;// taking into account margins.

					int shortcutWidth = 0;

					IRequiresControl barItem = item as IRequiresControl;
					if( barItem == null )
					{
						//if( item.Shortcut != Shortcut.None )
						{
							shortcutWidth = this.GetApproxWidth( g, item, item.ShortcutText );
						}
					}
					else
					{
						useOptimalTextAndShortcutColumnWidthsLogic = false;

						//if( hasText )
						{
							shortcutWidth = barItem.MinWidth - ArrowColumnWidth;
						}
					}

					bool maxWidthChanged = false;

					// Keep track of the max shortcut column width
					if( shortcutMaxWidth < shortcutWidth )
					{
						shortcutMaxWidth = shortcutWidth;
						shortcutMaxWidthItem = item;
						maxWidthChanged = true;
					}

					// Keep track of the max text column width
					if( textMaxWidth < textWidth )
					{
						textMaxWidth = textWidth;
						textMaxWidthItem = item;
						maxWidthChanged = true;
					}

					if( maxWidthChanged && useOptimalTextAndShortcutColumnWidthsLogic )
					{
						if( shortcutMaxWidthItem != textMaxWidthItem
							&& shortcutMaxWidth > 0 && textMaxWidthItem.Shortcut != Shortcut.None )
						{
							useOptimalTextAndShortcutColumnWidthsLogic = false;
						}
					}
					// Keep track of the max combined column width
					if( combinedMaxWidth < shortcutWidth + textWidth )
					{
						combinedMaxWidth = shortcutWidth + textWidth;
						combinedMaxWidthItem = item;
					}

					if( item.Shortcut != Shortcut.None
                        && shortcutMaxWidthItem != item )
						useOptimalTextAndShortcutColumnWidthsLogic = false;
				}
				g.Dispose();

				textMaxWidth += 4;
				shortcutMaxWidth += 4;
				combinedMaxWidth += 8;
				if (this.parentItem.ResizeGlyphToFit)
					Model.ColWidths[ArrowColumn] = textMaxWidth / 3;
				Model.ColWidths[ShortcutColumn] = shortcutMaxWidth;
				Model.ColWidths[TextColumn] = textMaxWidth;
			}
		}

		protected void ResizeRowHeights()
		{
			this.Model.Data.RowCount = this.Model.RowCount;

			this.Model.RowHeights.ResizeToFit( GridRangeInfo.Table() );
		}

		protected virtual void RefreshMenuGrid( bool bNoShow )
		{
			Model.BeginUpdate( BeginUpdateOptions.None );
			this.FreeObjectCache();

			this.ResizeRowHeights();

			this.ResizeColumns();
			this.TopRowIndex = 1;
			this.PerformLayout();
			Model.EndUpdate( false );
			this.Invalidate();
		}
		#endregion MENU_OPERATIONS
		#region UIEVENTS
		protected virtual void HideHighlight( GridRangeInfo range )
		{
			this.InvalidateRange( range );
			if( range != GridRangeInfo.Empty && this.parentItem.Items != null && this.parentItem.Items.Count > ( range.Top - 1 ) )
				this.parentItem.Items[range.Top - 1].PerformUnselected();
		}
		protected virtual void ShowHighlight( GridRangeInfo range )
		{
			this.ScrollCellInView( range.Top, this.GetCol( 1 ) );
			this.InvalidateRange( range );
			BarItem item = this.parentItem.Items[range.Top - 1];
			item.PerformSelected();
			BarItemsContainerAccessibleObject containerAccObject = this.AccessibilityObject as BarItemsContainerAccessibleObject;
			int childIndex = containerAccObject.GetAccessibleObjectIndexFromBarItem( item );
			if( childIndex != -1 )
				containerAccObject.NotifyClients( AccessibleEvents.Focus, childIndex );
		}
		#endregion UIEVENTS
		#region GRID_OVERRIDES
		protected bool IsSelectable( int rowIndex )
		{
			if( rowIndex <= 0 || Model.RowHeights[rowIndex] == 0 || Model.HideRows[rowIndex]
				|| this.parentItem.Items[rowIndex - 1].Text == "-"
				)
				return false;
			else
				return true;
		}
		protected bool ShouldDrawEnabled( BarItem item )
		{
			bool customizing = this.Customizing;
			// If the ListBarItem has no children then draw disabled
			if( item is ListBarItem && !customizing )
			{
				ListBarItem lbi = item as ListBarItem;
				if( lbi.ChildCaptions.Count == 0 )
					return false;
			}
			if( ( !item.Enabled && !customizing ) || item.Text == "-"
				|| ( item is StaticBarItem && !customizing ) )
				return false;
			else
				return true;
		}


		protected virtual void ModelQueryRowCount( object sender, GridRowColCountEventArgs e )
		{
			if( this.MenuBound )
			{
				e.Count = this.parentItem.Items.Count;
				e.Handled = true;
			}
		}

		protected virtual void ModelQueryRowHeight( object sender, GridRowColSizeEventArgs e )
		{
			int rowIndex = e.Index;
			if( rowIndex <= 0 )
			{
				e.Size = 0;
				e.Handled = true;
			}
			else if( this.MenuBound && rowIndex <= this.parentItem.Items.Count )
			{
				BarItem barItem = this.parentItem.Items[rowIndex - 1];
				if( !ShouldDrawVisible( this.parentItem, barItem, this.Customizing ) )
				{
					e.Handled = true;
					e.Size = 0;
				}
				if( barItem.Text == "-" )
				{
					e.Handled = true;
					e.Size = 3;
				}
			}
		}

		protected virtual bool MoveSelection( MoveHint hint )
		{
			bool success = false;
			switch( hint )
			{
				case MoveHint.moveFirst:
				{
					// First row
					int newRow = 0;
					// Skip rows that cannot be selected
					while( newRow <= Model.RowCount &&
						( !IsSelectable( newRow ) || ( this.parentItem.Items[newRow - 1] is StaticBarItem ) )
						)
						newRow++;

					if( newRow <= Model.RowCount )
					{
						this.HighlightRange = GridRangeInfo.Row( newRow );
						success = true;
					}
					break;
				}
				case MoveHint.moveLast:
				{
					// Last row
					int newRow = Model.RowCount;
					// Skip rows that cannot be selected
					while( newRow > 0 &&
						( !IsSelectable( newRow ) || this.parentItem.Items[newRow - 1] is StaticBarItem ) )
						newRow--;

					if( newRow > 0 )
					{
						this.HighlightRange = GridRangeInfo.Row( newRow );
						success = true;
					}
					break;
				}
				case MoveHint.moveNext:
				{
					// This portion of the case shares some common code with the derived class.
					if( this.HighlightRange == GridRangeInfo.Empty )
						success = this.MoveSelection( MoveHint.moveFirst );
					else
					{
						int newRow = this.HighlightRange.Top + 1;

						// Skip rows that cannot be selected
						while( newRow <= Model.RowCount &&
						( !IsSelectable( newRow ) || this.parentItem.Items[newRow - 1] is StaticBarItem ) )
							newRow++;

						if( newRow <= Model.RowCount )
						{
							this.HighlightRange = GridRangeInfo.Row( newRow );
							success = true;
						}
						else
							success = this.MoveSelection( MoveHint.moveFirst );
					}
					break;
				}
				case MoveHint.movePrevious:
				{
					if( this.HighlightRange == GridRangeInfo.Empty )
						success = this.MoveSelection( MoveHint.moveLast );
					else
					{
						int newRow = this.HighlightRange.Top - 1;

						// Skip rows that cannot be selected
						while( newRow > 0 &&
						( !IsSelectable( newRow ) || this.parentItem.Items[newRow - 1] is StaticBarItem ) )
							newRow--;

						if( newRow > 0 )
						{
							this.HighlightRange = GridRangeInfo.Row( newRow );
							success = true;
						}
						else
							success = this.MoveSelection( MoveHint.moveLast );
					}
					break;
				}
			}
			return success;
		}
		protected virtual bool ProcessKeyDown( Keys keyCode )
		{
			if( keyCode != Keys.Return && keyCode != Keys.Tab && this.CurrentCell.HasControlFocus )
				return false;

			bool bShift = ( Control.ModifierKeys & Keys.Shift ) != Keys.None;

			switch( keyCode )
			{
				case Keys.Tab:
				{
					if( bShift )
						return this.MoveSelection( MoveHint.movePrevious );
					else
						return this.MoveSelection( MoveHint.moveNext );
				}
				case Keys.Down:
				return this.MoveSelection( MoveHint.moveNext );
				case Keys.Up:
				return this.MoveSelection( MoveHint.movePrevious );
				case Keys.Home:
				return this.MoveSelection( MoveHint.moveFirst );
				case Keys.End:
				return this.MoveSelection( MoveHint.moveLast );
				case Keys.Return:
				{
					// Need IsModified check as well since the uneditable combo box
					// does not take focus but would still need to be "Stored" when Enter was hit.
					if( this.CurrentCell.HasControlFocus || this.CurrentCell.IsModified )
					{
						this.CurrentCell.Deactivate( false );
						return false;
					}
					break;
				}
			}
			return false;
		}

		// Need WantKeys: Setting WantKeys = false globally is an option but that requires additional work
		// to take care of Return and Esc. handling in combo boxes.
		protected override void OnScrollControlMouseDown( CancelMouseEventArgs e )
		{
			bool prevWantKeys = this.WantKeys;
			this.WantKeys = false;

			base.OnScrollControlMouseDown( e );

			this.WantKeys = prevWantKeys;
		}

		protected override bool IsInputKey( Keys keyData )
		{
			if( keyData == Keys.Enter || keyData == Keys.Escape )
				return true;

			return base.IsInputKey( keyData );
		}

		protected override void OnKeyDown( KeyEventArgs e )
		{
			if( e.Handled )
				return;

			e.Handled |= this.ProcessKeyDown( e.KeyCode & Keys.KeyCode );
		}
		void ModelQueryCoveredRange( object sender, GridQueryCoveredRangeEventArgs e )
		{
			GridRangeInfo range;
			OnQueryCoveredRange( e.RowIndex, e.ColIndex, out range );
			e.Range = range;
			e.Handled = true;
		}

		public virtual bool OnQueryCoveredRange( int rowIndex, int colIndex, out GridRangeInfo range )
		{
			range = GridRangeInfo.Empty;

			// If separator cell, cover the row
			if( rowIndex > 0 && this.parentItem != null &&
				this.parentItem.Items != null && rowIndex <= this.parentItem.Items.Count )
			{
				BarItem barItem = this.parentItem.Items[rowIndex - 1];
				int col = ( barItem.Text.Length > 0 ) ? ShortcutColumn : TextColumn;

				if( barItem.Text == "-"
					&& ( colIndex >= TextColumn && colIndex <= ArrowColumn ) )
				{
					range = GridRangeInfo.Cells( rowIndex, TextColumn, rowIndex, ArrowColumn );
					return true;
				}
				// Conver the text column and the shortcut column, if there is no shortcut text.
				else if( ( colIndex == ShortcutColumn || colIndex == TextColumn ) &&
					!( barItem is IRequiresControl ) )
				{
					if( barItem.ShortcutText == String.Empty )
					{
						range = GridRangeInfo.Cells( rowIndex, TextColumn, rowIndex, ShortcutColumn );
						return true;
					}
				}
				// Check for visibility in the 2 cases below, otherwise there are drawing problems.
				else if( ( colIndex == col || colIndex == ShortcutColumn || colIndex == ArrowColumn ) &&
					barItem is IRequiresControl
					&& ShouldDrawVisible( this.ParentItem, barItem, this.Customizing ) )
				{
					range = GridRangeInfo.Cells( rowIndex, col, rowIndex, ArrowColumn );
					return true;
				}
			}
			return false;
		}

		void ModelQueryCellInfo( object sender, GridQueryCellInfoEventArgs e )
		{
            e.Style.Borders.All = new GridBorder(GridBorderStyle.None);
			OnQueryCellInfo( e.RowIndex, e.ColIndex, e.Style, 0 );
			e.Handled = e.RowIndex >= 0;
		}

		protected virtual void UpdateStyleBasedOnItem( GridStyleInfo style, IRequiresControl barItemRequiringControl )
		{
			BarItem barItem = barItemRequiringControl as BarItem;
			ComboBoxBarItem item = barItem as ComboBoxBarItem;

			if( item != null )
			{
				style.CellType = "ComboBox_ListboxMode";
				style.ChoiceList = item.ChoiceList;
				style.ExclusiveChoiceList = !item.Editable;
			}
			else if( barItem is TextBoxBarItem )
			{
				style.CellType = "TextBox";
			}

			style.Clickable = barItem.Enabled;

			if( this.ThemesEnabled && IsVistaOS )
			{
				style.Borders.All = new GridBorder( GridBorderStyle.Solid, VistaMenuColors.SelBorderColor );
			}
			else
			{
				style.Borders.All = new GridBorder( GridBorderStyle.Solid, SystemColors.Control );
			}
		}

		protected virtual void SetTextColorAndFontOnStyle( BarItem BarItem, GridStyleInfo Style )
		{
			if( BarItem != null )
			{
				if( BarItem.CustomTextFont != null )
				{
					Style.Font = new GridFontInfo( BarItem.CustomTextFont );
				}
				if( BarItem.Enabled )
				{
					if( BarItem.CustomNormalTextColor != Color.Empty && this.SelectedItem != BarItem )
					{
						Style.TextColor = BarItem.CustomNormalTextColor;
					}
					else if( BarItem.CustomActiveTextColor != Color.Empty &&
						this.SelectedItem == BarItem )
					{
						Style.TextColor = BarItem.CustomActiveTextColor;
					}
				}
				else if( BarItem.CustomDisabledTextColor != Color.Empty )
				{
					Style.TextColor = BarItem.CustomDisabledTextColor;
				}
			}
		}

		public virtual bool OnQueryCellInfo( int rowIndex, int colIndex, GridStyleInfo style, int nType )
		{
			bool returnVal = false;

			if( rowIndex == 0 || !this.MenuBound )
				return returnVal;

			bool bRTL = this.IsRightToLeft();
			BarItem barItem = null;
			if( rowIndex > 0 )
				barItem = this.parentItem.Items[rowIndex - 1];

			// Col Styles
			if( rowIndex == -1 && colIndex != -1 )
			{
				if( colIndex == TextColumn )
				{
					// Text Column 
					style.CellType = "MenuTextCell";

					if( this.ThemesEnabled && IsVistaOS )
					{
						style.Interior = new BrushInfo( this.BackColor );
					}

					returnVal = true;
					// Model.ColStyles[TextColumn].CellType = "MenuTextCell";
				}
				else if( colIndex == IconColumn )
				{
					// Icon Column 
					style.CellType = "IconCell";

					if( this.ThemesEnabled && IsVistaOS )
					{
						style.Interior = new BrushInfo( this.BackColor );
					}
					else
					{
						style.Interior = GetColumnBrushInfo( this.Style );
					}

					returnVal = true;
				}
				else if( colIndex == ShortcutColumn )
				{
					// Shortcut column 
					style.HorizontalAlignment = bRTL ? GridHorizontalAlignment.Left : GridHorizontalAlignment.Right;
					style.CellType = "ShortcutCell";

					if( this.ThemesEnabled && IsVistaOS )
					{
						style.Interior = new BrushInfo( this.BackColor );
					}

					returnVal = true;
				}
				else if( colIndex == ArrowColumn )
				{
					// MenuGlyph Column
					style.CellType = "MenuGlyphCell";

					if( this.ThemesEnabled && IsVistaOS )
					{
						style.Interior = new BrushInfo( this.BackColor );
					}

					returnVal = true;
				}
			}
			// Row Styles
			else if( colIndex == -1 && rowIndex != -1 )
			{
				if( barItem is StaticBarItem )
				{
					style.Enabled = false;

					if( this.ThemesEnabled && IsVistaOS )
					{
						style.Interior = new BrushInfo( VistaMenuColors.SelBGColorDark );
					}
					else
					{
						style.Interior = GetStaticBrushInfo( this.Style );
					}

					returnVal = true;
				}
				else
				{
					Font font = style.Font.GdipFont;
					ProvideFontInfoEventArgs args = new ProvideFontInfoEventArgs( font );
					barItem.OnProvideFontInfo( args );
					if( font != args.Font )
					{
						this.SetStyleInfoFontFromGdipFont( style, args.Font );
						returnVal = true;
					}

					// Row base style
					if( !this.ShouldDrawEnabled( barItem ) )
					{
						style.Enabled = false;
						style.TextColor = ( barItem.CustomDisabledTextColor == Color.Empty ) ? MenuColors.DisabledMenuTextColorBase :
						barItem.CustomDisabledTextColor;
						returnVal = true;
					}
					else
					{
						style.TextColor = ( barItem.CustomNormalTextColor == Color.Empty ) ? MenuColors.MenuTextColor :
						barItem.CustomNormalTextColor;
					}

					if( this.HighlightRange.Top == rowIndex )
					{
						if( this.ThemesEnabled && IsVistaOS )
						{
							style.Interior = new BrushInfo( GradientStyle.Vertical, VistaMenuColors.SelBGColorLight, VistaMenuColors.SelBGColorDark );
						}
						else
						{
							if( this.ShouldDrawEnabled( barItem ) )
							{
								style.Interior = GetHighlightBrushInfo( this.Style );
							}
							else
							{
								style.Interior = new BrushInfo( MenuColors.MenuBGColor );
							}
						}

						returnVal = true;

						if( barItem.Enabled )
						{
							style.TextColor = ( barItem.CustomActiveTextColor == Color.Empty ) ? MenuColors.SelTextColor :
								barItem.CustomActiveTextColor;
						}
					}
				}
			}
			// Styles for individual cells
			else if( colIndex != -1 && rowIndex != -1 )
			{
				bool hasText = ( barItem.Text.Length > 0 );

				switch( colIndex )
				{
					case IconColumn:
					{
						// Otherwise the check mark cell seems to float into this one.
						style.CellValue = " ";
						if( barItem.Checked )
							style.CellValue = 1;
						else
							style.CellValue = 0;

						style.ImageIndex = ( barItem.PaintStyle == PaintStyle.TextOnlyInMenus ) ? -1 : barItem.ImageIndex;

						break;
					}

					case TextColumn:
					{
						style.CellValue = barItem.Text;
						style.HotkeyPrefix = barItem.HintViaHotKeyPrefix || barItem.ShowMnemonicUnderlinesAlways ? HotkeyPrefix.Show : HotkeyPrefix.Hide;

						if( barItem != null )
						{
							style.SetValue( GridStyleInfoStore.HorizontalAlignmentProperty, (int)barItem.TextAlignment );
						}

						this.SetTextColorAndFontOnStyle( barItem, style );

						// Its a separator cell
						if( style.CellValue.ToString() == "-" )
						{
							style.CellType = "SeparatorCell";
						}
						else
						{
							// This is necessary ever since we use ResizeToFit - to support custom fonts.
							style.BorderMargins.Top = 2;
							style.BorderMargins.Bottom = 3;
						}

						if( !hasText )
						{
							CellRequiresControl( style, barItem );
						}

						break;
					}

					case ShortcutColumn:
					{
						if( !CellRequiresControl( style, barItem ) )
						{
							if( !( barItem is StaticBarItem ) )
							{
								Shortcut shortcut = barItem.Shortcut;

								//if( shortcut != Shortcut.None )
								{
									style.CellValue = barItem.ShortcutText;
								}

								style.HorizontalAlignment = GridHorizontalAlignment.Right;

								this.SetTextColorAndFontOnStyle( barItem, style );
							}
						}

						break;
					}

					case ArrowColumn:
					{
						style.CellValue = ( barItem is ParentBarItem || barItem is DropDownBarItem ) ? 1 : 0;
						break;
					}
				}

				if( this.ThemesEnabled && IsVistaOS
					&& colIndex < TextColumn && !barItem.IsRecentlyUsedItem
					&& this.parentItem.UsePartialMenus
					&& ( this.parentItem.Manager == null || this.parentItem.Manager.UsePartialMenus )
					&& this.HighlightRange.Top != rowIndex )
				{
					if( this.Style != VisualStyle.Office2003 )
						style.Interior = new BrushInfo( MenuColors.ExpandedMenuStripBackColor );
					else
					{
						if( bRTL )
						{
							style.Interior = new BrushInfo( GradientStyle.Horizontal,
								Office2003Colors.MenuExpandedItemsMarginColorRight, Office2003Colors.MenuExpandedItemsMarginColorLeft );
						}
						else
						{
							style.Interior = new BrushInfo( GradientStyle.Horizontal,
								Office2003Colors.MenuExpandedItemsMarginColorLeft, Office2003Colors.MenuExpandedItemsMarginColorRight );
						}
					}
				}
				returnVal = true;
			}

			return returnVal;
		}

		private bool CellRequiresControl( GridStyleInfo style, BarItem barItem )
		{
			bool result = false;
			IRequiresControl barItemWithControl = barItem as IRequiresControl;

			if( barItemWithControl != null )
			{
				style.CellValue = barItemWithControl.Value;
				style.Interior = new BrushInfo( MenuColors.MenuBGColor );
				style.HorizontalAlignment = GridHorizontalAlignment.Left;

				this.UpdateStyleBasedOnItem( style, barItem as IRequiresControl );
				result = true;
			}

			return result;
		}
		protected override void DrawInvertCell( Graphics g, int rowIndex, int colIndex, Rectangle rectItem, bool inPaint )
		{
			m_bInvertRect = false;
		}

		protected override void OnDrawCellFrameAppearance( GridDrawCellBackgroundEventArgs e )
		{
			base.OnDrawCellFrameAppearance( e );

			if( this.Style == VisualStyle.Office2007 &&
				!( this.ThemesEnabled && IsVistaOS ) )
			{
				Rectangle rect = e.TargetBounds;

				// draw column of the menu
				if( e.Range.Left == IconColumn )
				{
					bool bRTL = IsRightToLeft();
					Office2007MenuPainter.DrawMenuColumn( e.Graphics, rect, bRTL );
				}

				// draw highlighted cell with Office2007 visual style
				if( this.HighlightRange.Top == e.Range.Top )
				{
					Office2007MenuPainter.DrawMenuItemHighlightBackground( e.Graphics, rect );
					e.Cancel = true;
				}
			}
            else if (this.Style == VisualStyle.Office2010 &&
                !(this.ThemesEnabled && IsVistaOS))
            {
                Rectangle rect = e.TargetBounds;

                // draw column of the menu
                if (e.Range.Left == IconColumn)
                {
                    bool bRTL = IsRightToLeft();
                    Office2010MenuPainter.DrawMenuColumn(e.Graphics, rect, bRTL);
                }

                // draw highlighted cell with Office2007 visual style
                if (this.HighlightRange.Top == e.Range.Top)
                {
                    Office2010MenuPainter.DrawMenuItemHighlightBackground(e.Graphics, rect);
                    e.Cancel = true;
                }
            }
            else if (this.Style == VisualStyle.Metro)
            {
                Rectangle rect = e.TargetBounds;
                bool bRTL = IsRightToLeft();
                MetroMenuPainter.DrawMetroMenuColumn(e.Graphics, rect, bRTL);
                if (this.HighlightRange.Top == e.Range.Top)
                {
                    MetroMenuPainter.DrawMenuItemHighlightBackground(e.Graphics, rect);
                    e.Cancel = true;
                }
            }
		}

		protected override void OnPaint( PaintEventArgs pevent )
		{
			UpdateColorScheme();

			Graphics g = pevent.Graphics;
			SmoothingMode savedMode = g.SmoothingMode;

			Rectangle selectedCellsBounds = this.RangeInfoToRectangle( this.HighlightRange, GridRangeOptions.None );
			selectedCellsBounds.Width -= 1;
			selectedCellsBounds.Height -= 1;

			if( !this.MenuBound )
				return;

			if( !this.HighlightRange.IsEmpty && this.ThemesEnabled && IsVistaOS )
			{
				GraphicsPath border = DrawingUtils.GetRoundedRectangle( selectedCellsBounds, MenuGridControlBase.DEF_BORDERRADIUS );

				g.SmoothingMode = SmoothingMode.AntiAlias;

				using( Pen pen = new Pen( VistaMenuColors.SelBorderColor ) )
				{
					//this.RefreshRange
					g.DrawPath( pen, border );
				}
                border.Dispose();
			}

			base.OnPaint( pevent );

			if( !this.HighlightRange.IsEmpty && !( this.ThemesEnabled && IsVistaOS ) )
			{
				if( this.Style == VisualStyle.Office2007 )
				{
					// draws highlighted bodred with Office2007 visual style.
					Office2007MenuPainter.DrawMenuItemHighlightBorder( g, selectedCellsBounds );
				}
                else if (this.Style == VisualStyle.Office2010)
                {
                    // draws highlighted bodred with Office2010 visual style.
                    Office2010MenuPainter.DrawMenuItemHighlightBorder(g, selectedCellsBounds);
                }
                else if(this.Style != VisualStyle.Metro) 
				{
					Pen pen = GetBorderPen( this.Style );
					g.DrawRectangle( pen, selectedCellsBounds );
					pen.Dispose();
				}
			}

			g.SmoothingMode = savedMode;
		}

		protected override void OnThemeChanged( EventArgs e )
		{
			base.OnThemeChanged( e );

			if( this.ThemesEnabled && IsVistaOS )
			{
				this.BackColor = VistaMenuColors.BackgroundColor;
			}
			else
			{
				this.BackColor = Color.White;
			}
		}

		#endregion GRID_OVERRIDES

		#region UTILS

		internal bool IsItemDropDownStyle( BarItem barItem )
		{
            return (barItem is IParentBarItem && ((IParentBarItem)barItem).ParentStyle == ParentBarItemStyle.DropDown) ||
                barItem is ComboBoxBarItem;
		}

		/// <summary>
		/// Gets pen for border of the selected item amenably with VisualStyle.
		/// </summary>
		private Pen GetBorderPen( VisualStyle style )
		{
			Pen pen = null;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					pen = new Pen( Office2003Colors.SelBorderColor );
					break;
				}
				case VisualStyle.VS2005:
				{
					pen = new Pen( VS2005Colors.MenuSelectedItemBorderColor );
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					pen = new Pen( Office2007OutlookColors.MenuSelectedItemBorderColor );
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
		/// Get BrushInfo for column amenably with VisualStyle.
		/// </summary>
		private BrushInfo GetColumnBrushInfo( VisualStyle style )
		{
			BrushInfo brushInfo = null;
			bool bRTL = this.IsRightToLeft();

			if( this.ThemesEnabled && IsVistaOS )
			{
				brushInfo = new BrushInfo( this.BackColor );
			}
			else
			{
				switch( style )
				{
					case VisualStyle.Office2003:
					{
						if( bRTL )
						{
							brushInfo = new BrushInfo( GradientStyle.Horizontal,
								Office2003Colors.MenuMarginColorDark, Office2003Colors.MenuMarginColorLight );
						}
						else
						{
							brushInfo = new BrushInfo( GradientStyle.Horizontal,
								Office2003Colors.MenuMarginColorLight, Office2003Colors.MenuMarginColorDark );
						}
						break;
					}

					case VisualStyle.Office2007Outlook:
					{
						brushInfo = new BrushInfo( Office2007OutlookColors.MenuColumnStyleColor );
						break;
					}

					case VisualStyle.VS2005:
					{
						if( bRTL )
						{
							brushInfo = new BrushInfo( GradientStyle.Horizontal,
								VS2005Colors.MenuColumnStyleDarkColor, VS2005Colors.MenuColumnStyleLightColor );
						}
						else
						{
							brushInfo = new BrushInfo( GradientStyle.Horizontal,
								VS2005Colors.MenuColumnStyleLightColor, VS2005Colors.MenuColumnStyleDarkColor );
						}
						break;
					}

                    case VisualStyle.Metro:
                    {
                        brushInfo = new BrushInfo(Color.FromArgb(90, ColorTranslator.FromHtml("#119EDA")));
                        break;
                    }
					default:
					{
						brushInfo = new BrushInfo( MenuColors.MenuLeftStripColor );
						break;
					}
				}
			}

			return brushInfo;
		}


		/// <summary>
		/// Get BrushInfo for highlighted item amenably with VisualStyle.
		/// </summary>
		private BrushInfo GetHighlightBrushInfo( VisualStyle style )
		{
			BrushInfo brushInfo = null;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					brushInfo = new BrushInfo( Office2003Colors.SelColor );
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					brushInfo = new BrushInfo( Office2007OutlookColors.MenuSelectedItemColor );
					break;
				}
				case VisualStyle.VS2005:
				{
					brushInfo = new BrushInfo( VS2005Colors.MenuSelectedItemColor );
					break;
				}
				default:
				{
					brushInfo = new BrushInfo( MenuColors.SelColor );
					break;
				}
			}

			return brushInfo;
		}


		/// <summary>
		/// Get BrushInfo for static item amenably with VisualStyle.
		/// </summary>
		private BrushInfo GetStaticBrushInfo( VisualStyle style )
		{
			BrushInfo brushInfo = null;

			if( this.ThemesEnabled && IsVistaOS )
			{
				brushInfo = new BrushInfo( VistaMenuColors.SelBGColorDark );
			}
			else
			{
				switch( style )
				{
					case VisualStyle.Office2003:
					{
						brushInfo = new BrushInfo( Office2003Colors.MenuMarginColorDark );
						break;
					}

					case VisualStyle.Office2007Outlook:
					{
						brushInfo = new BrushInfo( Office2007OutlookColors.MenuColumnStyleColor );
						break;
					}

					case VisualStyle.VS2005:
					{
						brushInfo = new BrushInfo( VS2005Colors.MenuColumnStyleDarkColor );
						break;
					}

					default:
					{
						brushInfo = new BrushInfo( MenuColors.MenuLeftStripColor );
						break;
					}
				}
			}

			return brushInfo;
		}

		/// <summary>
		/// Updates color schemes.
		/// </summary>
		private void UpdateColorScheme()
		{
			MenuColors.UpdateMenuColors();
			Office2003Colors.UpdateMenuColors();
			VS2005Colors.UpdateMenuColors();

			Office2007Theme theme = ( parentItem != null ) ?
				parentItem.Office2007Theme : Office2007Theme.Blue;
            Office2010Theme theme1 = (parentItem != null) ?
                parentItem.Office2010Theme : Office2010Theme.Blue;
			Office2007OutlookColors.UpdateMenuColors( theme );
            Office2010Colors.DefaultTheme = theme1;
			Office2007Colors.DefaultTheme = theme;
			Office2007MenuPainter.ColorTable = this.Office2007ColorTable;
		}

		internal bool ForcedVistaStyle
		{
			set
			{
				if( value )
				{
					m_bIsVistaOS = true;
				}
				else
				{
					m_bIsVistaOS = ( Environment.OSVersion.Version.Major >= 6 );
				}
			}
		}

		#endregion UTILS

		private void Model_QueryColCount( object sender, GridRowColCountEventArgs e )
		{
			e.Count = this.MenuColCount;
			e.Handled = true;
		}

		#region IMessageFilter Members

		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == NativeMethods.WM_KEYDOWN)
			{
				Keys keyCode = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;
				if (keyCode == Keys.Tab && (this.SelectedItem as DropDownBarItem) == null)
					return this.ProcessKeyDown(keyCode);
			}
			return false;
		}

		#endregion
	}

	interface IFocusableRenderer
	{
		bool IsRelatedControl( Control control );
	}

	[Documentation.DocumentationExclude()]
	[EditorBrowsable( EditorBrowsableState.Never )]
	[ToolboxItem( false )]
	public class CustomizingPopupMenu: PopupMenu
	{
		protected ParentBarItem customizationMenu;
		protected IBarItemContainer parentItem;
		protected BarItem selectedItem;
		private TextBoxBarItem nameEditorItem;
		private BarItem deleteItem, defaultPaintStyle, textOnlyAlways, textOnlyInMenus, imageAndText, beginAGroup, resetItem, imageBarItem;
		protected CustomizationDndHelper helper;
		internal BarManager dummyManager;
		private bool ignoreEvents = false;

		public CustomizingPopupMenu()
		{
			this.dummyManager = new BarManager();
			this.InitCustomizationMenu();
		}
		protected virtual void InitCustomizationMenu()
		{
			this.customizationMenu = new ParentBarItem();
			this.dummyManager.Items.Add( this.customizationMenu );
			this.ParentBarItem = this.customizationMenu;

			// Init items
			this.deleteItem = new BarItem( SR.GetString( SR.DeleteMenuItemText, this) );
			this.defaultPaintStyle = new BarItem( SR.GetString( SR.DefaultMenuItemText, this) );
			this.textOnlyAlways = new BarItem( SR.GetString( SR.TextOnlyAlways, this) );
			this.textOnlyInMenus = new BarItem( SR.GetString( SR.TextOnlyInMenus, this) );
			this.imageAndText = new BarItem( SR.GetString( SR.ImageAndText, this) );
			this.nameEditorItem = new TextBoxBarItem( SR.GetString( SR.NameCaption, this) );
			this.beginAGroup = new BarItem( SR.GetString( SR.BeginAGroup, this ) );
			this.resetItem = new BarItem( SR.GetString( SR.ResetBarItem, this) );
			this.imageBarItem=new BarItem("Change Image");

			this.nameEditorItem.MinWidth = 100;

			// Setup handlers
			this.resetItem.Click += new EventHandler( this.CustomizingMenuItemClicked );
			this.deleteItem.Click += new EventHandler( this.CustomizingMenuItemClicked );
			this.nameEditorItem.PropertyChanged
				+= new SyncfusionPropertyChangedEventHandler( this.NameEditorValueChanged );
			this.defaultPaintStyle.Click += new EventHandler( this.CustomizingMenuItemClicked );
			this.textOnlyAlways.Click += new EventHandler( this.CustomizingMenuItemClicked );
			this.textOnlyInMenus.Click += new EventHandler( this.CustomizingMenuItemClicked );
			this.imageAndText.Click += new EventHandler( this.CustomizingMenuItemClicked );
			this.beginAGroup.Click += new EventHandler( this.CustomizingMenuItemClicked );
            this.imageBarItem.Click += new EventHandler(this.CustomizingMenuItemClicked);

			this.customizationMenu.Items.AddRange( new BarItem[]{this.resetItem, this.deleteItem,this.imageBarItem, this.nameEditorItem, this.defaultPaintStyle,
																   this.textOnlyAlways, this.textOnlyInMenus, this.imageAndText, this.beginAGroup} );
			this.customizationMenu.BeginGroupAt( this.nameEditorItem );
			this.customizationMenu.BeginGroupAt( this.defaultPaintStyle );
			this.customizationMenu.BeginGroupAt( this.beginAGroup );

			this.customizationMenu.BeforePopup += new CancelEventHandler( this.CustomizeMenuBeforePopup );
			this.customizationMenu.Popup += new EventHandler( this.CustomizeMenuPopup );
		}

		protected virtual void CustomizingMenuItemClicked( object sender, EventArgs e )
		{
			if( this.ignoreEvents )
				return;

			PaintStyle newPaintStyle = this.selectedItem.PaintStyle;
			if( sender == this.deleteItem )
				this.helper.RemoveItem( this.selectedItem );
			else if( sender == this.resetItem )
				this.helper.ResetItem( this.selectedItem, true );
			else if( sender == this.defaultPaintStyle )
				newPaintStyle = PaintStyle.Default;
			else if( sender == this.textOnlyAlways )
				newPaintStyle = PaintStyle.TextOnly;
			else if( sender == this.textOnlyInMenus )
				newPaintStyle = PaintStyle.TextOnlyInMenus;
			else if( sender == this.imageAndText )
				newPaintStyle = PaintStyle.ImageAndText;
			else if( sender == this.beginAGroup )
			{
				//Toggle BeginGroup
				bool beginGroup = false;
				if( !this.parentItem.IsGroupBeginning( this.selectedItem ) )
					beginGroup = true;

				if( !this.parentItem.Manager.DesignMode )
					this.selectedItem.Manager.MainFrameBarManager.RecordModifiedGrouping
						( this.selectedItem, beginGroup, this.parentItem );
				else
				{
					if( beginGroup )
						this.parentItem.BeginGroupAt( this.selectedItem );
					else
						this.parentItem.RemoveGroupAt( this.selectedItem );
				}
			}
            else if (sender == this.imageBarItem)
            {
                OpenFileDialog openImageFile = new OpenFileDialog();

                openImageFile.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG)|*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG";
                if (openImageFile.ShowDialog() == DialogResult.OK)
                {
                    ImageExt image = new ImageExt(Image.FromFile(openImageFile.FileName));
                    if(this.selectedItem != null)
                        this.selectedItem.Image = image;
                }

                this.selectedItem.Manager.MainFrameBarManager.RemovePreviouslyAddedChangeImageInfo(this.selectedItem);

                if (!this.parentItem.Manager.DesignMode)
                    this.selectedItem.Manager.MainFrameBarManager.RecordChangedImage(this.selectedItem, this.selectedItem.Image);
				openImageFile.Dispose();
            }

			if( newPaintStyle != this.selectedItem.PaintStyle )
			{
				if( !this.parentItem.Manager.DesignMode )
					this.selectedItem.Manager.MainFrameBarManager.RecordModifiedPaintStyle
						( this.selectedItem, newPaintStyle );
				else
					this.selectedItem.PaintStyle = newPaintStyle;
			}
		}

		private void NameEditorValueChanged( object sender, SyncfusionPropertyChangedEventArgs e )
		{
			if( this.ignoreEvents )
				return;

			if( e.PropertyName == "TextBoxValue" )
			{
				string newCaption = this.nameEditorItem.TextBoxValue;
				if( !this.parentItem.Manager.DesignMode )
					this.selectedItem.Manager.MainFrameBarManager.RecordModifiedCaption( this.selectedItem, newCaption );
				else
					this.selectedItem.Text = newCaption;
			}
		}


		private void CustomizeMenuBeforePopup( object sender, CancelEventArgs e )
		{
			this.ignoreEvents = true;
			if( !this.parentItem.Manager.DesignMode
				&& this.parentItem.Manager.MainFrameBarManager != null
				&& !this.parentItem.Manager.MainFrameBarManager.AllowUserRenaming )
			{
				// Adding and removing to workaround the drawing bug with a hidden combo box bar item.
				this.customizationMenu.Items.Remove( this.nameEditorItem );
				//this.nameEditorItem.Visible = false;
			}
			else
			{
				if( this.customizationMenu.Items.IndexOf( this.nameEditorItem ) == -1 )
				{
					this.customizationMenu.Items.Insert( this.customizationMenu.Items.IndexOf( this.deleteItem ) + 1,
						this.nameEditorItem );
					this.customizationMenu.BeginGroupAt( this.nameEditorItem );
				}
				//this.nameEditorItem.Visible = true;
			}
			this.ignoreEvents = false;
		}
		private void CustomizeMenuPopup( object sender, EventArgs e )
		{
			this.ignoreEvents = true;
			if( this.parentItem.Manager.DesignMode )
				this.resetItem.Visible = false;
			else
				this.resetItem.Visible = true;


			this.nameEditorItem.TextBoxValue = selectedItem.Text;
			this.defaultPaintStyle.Checked = false;
			this.textOnlyAlways.Checked = false;
			this.textOnlyInMenus.Checked = false;
			this.imageAndText.Checked = false;
			this.beginAGroup.Checked = false;

			switch( selectedItem.PaintStyle )
			{
				case PaintStyle.Default:
				this.defaultPaintStyle.Checked = true; break;
				case PaintStyle.TextOnly:
				this.textOnlyAlways.Checked = true; break;
				case PaintStyle.TextOnlyInMenus:
				this.textOnlyInMenus.Checked = true; break;
				case PaintStyle.ImageAndText:
				this.imageAndText.Checked = true; break;
			}
			if( this.parentItem.IsGroupBeginning( selectedItem ) )
				this.beginAGroup.Checked = true;

			this.ignoreEvents = false;
		}

		public void Show( Control control, Point pos, BarItem selectedItem,
			IBarItemContainer parent, CustomizationDndHelper helper )
		{
			// Don't have to show context menus for these items.
			if( selectedItem is StandAloneBarItem )
				return;

			this.selectedItem = selectedItem;
			this.parentItem = parent;
			this.helper = helper;

			base.Show( control, pos );
		}

		public bool IsPopupUI( Control control )
		{
			if( this.childMenuUI != null )
				return control == this.childMenuUI;
			else
				return false;
		}

		public override void ChildClosing( IPopupChild childUI, PopupCloseType popupCloseType )
		{
			if( this.GetPopupParentControl() is MenuGrid )
			{
				MenuGrid menuGrid = (MenuGrid)this.GetPopupParentControl();
				if( menuGrid.IsShowing() )
					PopupManager.SetCurrentPopupClient( menuGrid, true, false );
			}
			base.ChildClosing( childUI, popupCloseType );
		}
	}

	[ToolboxItem( false ),
	Syncfusion.Documentation.DocumentationExclude()
	]
	public class MenuGridControlCustomizable: MenuGridControlBase, IDndTrackingControl
	{
		protected internal CustomizationDndHelper dndHelper;
		private CustomizingPopupMenu customizationPopup;
		internal DragDropEffects allowedDragEffects;
		internal bool allowDropOnSelf = false;

		public MenuGridControlCustomizable( bool allowDropOnSelf )
		{
			this.allowDropOnSelf = allowDropOnSelf;
			// Moving this to MenuBound - so that AllowDrop will not be called within the constructor -
			// otherwise various reflection based operations fail - licensing mechanism for example.
			// this.AllowDrop = true;
			// Init on demand for perf. reasons.
			//this.customizationPopup = new CustomizingPopupMenu();
			allowedDragEffects = DragDropEffects.None | DragDropEffects.Copy | DragDropEffects.Move;
			this.dndHelper = new CustomizationDndHelper( this );
		}
		protected bool IsCustomizationPopupAvailable
		{
			get { return this.customizationPopup != null; }
		}

		protected CustomizingPopupMenu CustomizationPopup
		{
			get
			{
				if( this.customizationPopup == null )
					this.customizationPopup = new CustomizingPopupMenu();

				return this.customizationPopup;
			}
		}
		protected virtual DragDropEffects GetAllowedDragEffects()
		{
			return this.allowedDragEffects;
		}

		protected override bool MenuBound
		{
			get { return base.MenuBound; }
			set
			{
				if( base.MenuBound != value )
				{
					if( Application.OleRequired() == System.Threading.ApartmentState.STA )
						this.AllowDrop = true;

					base.MenuBound = value;
					if( value == false && this.dndHelper != null && !this.dndHelper.Dragging )
						this.dndHelper.ParentItem = null;

					if( value )
					{
						if( this.parentItem.Manager != null && this.parentItem.Manager.DesignMode )
							this.CustomizationPopup.dummyManager.SetUseHooksForMenus( true );
					}
					else if( this.customizationPopup != null )
						this.customizationPopup.dummyManager.SetUseHooksForMenus( false );
				}
				// Helps while Jiting
				else
				{
					if( Application.OleRequired() == System.Threading.ApartmentState.STA )
						this.AllowDrop = false;

					base.MenuBound = value;
				}
			}
		}

		protected virtual void OnStartDragging()
		{
		}

		protected virtual bool ShouldRemoveItemOnMove()
		{
			return false;
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			if( this.mouseDownPoint != Point.Empty && this.mouseDownPoint == new Point( e.X, e.Y ) )
				return;

			this.mouseDownPoint = Point.Empty;

			// Start a drag and drop, when pressed and moved.
			if( this.Customizing && e.Button == MouseButtons.Left
				&& this.HighlightRange.Top > 0 )
			{
				int hitItem = this.HitTestBarItemIndex( new Point( e.X, e.Y ) );
				BarItem selectedItem = this.parentItem.Items[this.HighlightRange.Top - 1];
				if( this.mouseDownItem != -1 &&
					hitItem == this.mouseDownItem && this.parentItem.Manager.CanStartDragging( selectedItem ) )
				{
					this.OnStartDragging();
					this.ValidateDndHelper();
					this.dndHelper.Dragging = true;
					BarItemDndData dndData = new BarItemDndData( selectedItem, this.parentItem );
					DragDropEffects dropEffect
						= this.DoDragDrop( new DataObject( typeof( BarItem ).FullName, dndData ),
											this.GetAllowedDragEffects() );
					// If source was not the same as destination
					if( dndHelper.Dragging )
					{
						if( dropEffect == DragDropEffects.Move && dndData.ItemDestination != dndData.ItemParent
							&& this.ShouldRemoveItemOnMove() )
						{
							this.dndHelper.RemoveItem( selectedItem );
						}
						this.dndHelper.Dragging = false;
					}
					this.ResetDragging();
				}
			}
			else
				base.OnMouseMove( e );
		}
		protected virtual void ValidateDndHelper()
		{
			this.dndHelper.ParentItem = this.parentItem;
		}

		protected virtual void UpdateSelectionOnDragging( Point ptClMouse )
		{
			GridRangeInfo range = this.PointToRangeInfo( ptClMouse, -1 );
			if( range.IsEmpty )
				return;
			int row = range.Top;
			int col = range.Left;

			if( row > 0 && row <= this.Model.RowCount &&
				this.parentItem.Items[row - 1] is ParentBarItem &&
				this.ShouldDrawEnabled( this.parentItem.Items[row - 1] ) )
			{
				// Update highlight range
				if( row <= Model.RowCount && col <= Model.ColCount
					&& this.IsSelectable( row ) )
					this.HighlightRange = GridRangeInfo.Rows( row, row );
				else
					this.HighlightRange = GridRangeInfo.Empty;
			}
			else
			{
				this.HighlightRange = GridRangeInfo.Empty;
			}
		}

		protected override void OnDragOver( DragEventArgs drgevent )
		{
			if( this.Customizing && this.allowDropOnSelf )
			{
				this.UpdateSelectionOnDragging( this.PointToClient( new Point( drgevent.X, drgevent.Y ) ) );

				this.ValidateDndHelper();

				this.dndHelper.OnDragOver( drgevent );
			}
		}

		protected override void OnDragDrop( System.Windows.Forms.DragEventArgs e )
		{
			base.OnDragDrop( e );
			if( this.Customizing && this.allowDropOnSelf )
			{
				this.ValidateDndHelper();
				this.dndHelper.OnDragDrop( e );
				// Doing this will let me know that source is same as destination (when DoDragDrop returns)
				this.dndHelper.Dragging = false;
			}
		}
		protected override void OnDragLeave( EventArgs e )
		{
			base.OnDragLeave( e );
			if( this.Customizing && this.allowDropOnSelf )
			{
				if( dndHelper != null )
					this.dndHelper.Reset();
			}
		}
		protected override void OnPaint( PaintEventArgs pe )
		{
			base.OnPaint( pe );

			if( !this.MenuBound )
				return;

			if( this.dndHelper != null )
				this.dndHelper.PaintCueCursor( pe.Graphics );
		}
		protected override void OnGiveFeedback( GiveFeedbackEventArgs gfbevent )
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
			return this;
		}

		public new bool DesignMode
		{
			get
			{
				if( this.parentItem != null && this.parentItem.Manager != null
					&& this.parentItem.Manager.DesignMode )
					return true;
				else
					return false;
			}
		}

		public bool CanDropItem( BarItem item )
		{
			ParentBarItem parent = this.dndHelper.ParentItem as ParentBarItem;
			return parent == null || parent.ShouldAddItem( item );
		}
		public BarItem HitTestBarItem( Point mousePosition, ref bool beforeOrAfter )
		{
			GridRangeInfo range = this.PointToRangeInfo( mousePosition, -1 );
			if( range.IsEmpty )
				return null;
			int row = range.Top;
			int col = range.Left;

			if( row > 0 && row <= this.Model.RowCount /*&& this.ShouldDrawEnabled((BarItem)this.parentItem.Items[row - 1])*/)
			{
				if( row <= Model.RowCount && col <= Model.ColCount
					&& this.IsSelectable( row ) )
				{
					BarItem selected = this.parentItem.Items[row - 1] as BarItem;
					Rectangle itemBounds = this.GetBoundsOf( selected );
					if( mousePosition.Y - itemBounds.Top
						> itemBounds.Bottom - mousePosition.Y )
						beforeOrAfter = false;
					else
						beforeOrAfter = true;

					return selected;
				}
				else if( row == 1 && this.parentItem.Items[row - 1].Text == "-" )
				{
					// The first item is a separator and mouse is hovering over it
					beforeOrAfter = true;
					return null;
				}
			}
			return null;
		}

		public Rectangle GetCueRect( BarItem barItem, bool beforeOrAfter )
		{
			if( barItem == null && beforeOrAfter )
				// Inserting before the first separator
				return new Rectangle( 0, 0, this.Width, 7 );

			Rectangle itemBounds = this.GetBoundsOf( barItem );
			if( beforeOrAfter )
				return new Rectangle( itemBounds.Left, itemBounds.Top, itemBounds.Width, 7 );
			else
				return new Rectangle( itemBounds.Left, itemBounds.Bottom - 8, itemBounds.Width, 7 );
		}

		#endregion IDndTrackingControl_Override
	}
	[ToolboxItem( false ),
	Syncfusion.Documentation.DocumentationExclude()
	]
	public class MenuGrid: MenuGridControlCustomizable, IPopupParent, IMouseHookHLProcClient,
		IKeyboardProcHookClient, IPopupControlContainer, IIgnoreWorkingArea
	{
        protected ToolTip toolTip = new ToolTip ();
		class MenuCloseEventListener
		{
			MenuGrid grid;
			Control targetControl;
			private Form dropDownHost;
			internal bool ignoreDeactivate = false;

			public MenuCloseEventListener( MenuGrid grid ) { this.grid = grid; }

			public Control TargetControl
			{
				get { return this.targetControl; }
				set
				{
					if( this.targetControl != value )
					{
						if( this.targetControl != null )
							Release();
						this.targetControl = value;
						if( this.targetControl != null )
							Attach();
					}
				}
			}
			public Form DropDownHost
			{
				get { return this.dropDownHost; }
				set
				{
					if( this.dropDownHost != value )
					{
						if( this.dropDownHost != null )
						{
							this.dropDownHost.Deactivate -= new EventHandler( this.FormDeactivate );
						}

						this.dropDownHost = value;

						if( this.dropDownHost != null )
						{
							this.dropDownHost.Deactivate += new EventHandler( this.FormDeactivate );
						}
					}
				}
			}

			protected internal bool IsCustomizing
			{
				get
				{
					bool bisCustomizing = false;
					if( grid != null )
					{
						bisCustomizing = grid.IsCustomizing;

					}

					return bisCustomizing;
				}
			}
			private void Attach()
			{
				if( this.targetControl != null )
				{
					Form parentForm = this.targetControl.FindForm();

					// Don't have to listen to these events if the parent is a MenuGridHost (will be case when user customizes with the context menu).
					if( parentForm != null && !( parentForm is MenuGridHost ) )
					{
						bool bisCustomizing = IsCustomizing;

						if( !bisCustomizing )
						{
							parentForm.Resize += new EventHandler( this.MenuCloseEvent );
						}

						parentForm.Move += new EventHandler( this.MenuCloseEvent );
						parentForm.Deactivate += new EventHandler( this.FormDeactivate );
						if( parentForm.ParentForm != null )
						{
							parentForm.ParentForm.Move += new EventHandler( this.MenuCloseEvent );
							parentForm.ParentForm.Deactivate += new EventHandler( this.FormDeactivate );
						}
					}
				}
			}
			private void Release()
			{
				if( this.targetControl != null )
				{
					Form parentForm = this.targetControl.FindForm();

					if( parentForm != null )
					{
						bool bisCustomizing = IsCustomizing;

						if( !bisCustomizing )
						{
							parentForm.Resize -= new EventHandler( this.MenuCloseEvent );
						}

						parentForm.Move -= new EventHandler( this.MenuCloseEvent );
						parentForm.Deactivate -= new EventHandler( this.FormDeactivate );
						if( parentForm.ParentForm != null )
						{
							parentForm.ParentForm.Move -= new EventHandler( this.MenuCloseEvent );
							parentForm.ParentForm.Deactivate -= new EventHandler( this.FormDeactivate );
						}
					}
				}
			}
			private void MenuCloseEvent( object sender, EventArgs e )
			{
				this.grid.HidePopup( PopupCloseType.Deactivated );
			}
			private void FormDeactivate( object sender, EventArgs e )
			{
				this.ValidateActiveState( sender );
			}
			private void ValidateActiveState( object sender )
			{
				// Form.ActiveForm == old active form when a dialog is about to be shown! 
				// So we want the menu to be hidden at this point.
				if( !this.grid.MenuBound ||
					this.ignoreDeactivate ||
					//Form.ActiveForm == sender ||
					( Form.ActiveForm != null &&
					this.grid.IsRelatedControl( Form.ActiveForm, true ) ) )
					return;
				else
					this.grid.HidePopup( PopupCloseType.Deactivated );
			}
		}

		#region TEMPORARY_STATE_MEMBERS
		private bool dontShowChild = false;
		private MenuCloseEventListener listener = null;
		private bool allowCurrentCellActivating = false;
		// This is necessary since the
		static Timer timer;
		static bool disablePartialMenusModeTemporarily = false;
		static bool DisablePartialMenusModeTemporarily
		{
			get { return disablePartialMenusModeTemporarily; }
			set
			{
				// If false, set it false only after 5 seconds
				if( value == false )
					StartPartialMenusTimer();
				else
				{
					StopPartialMenusTimer();
					disablePartialMenusModeTemporarily = true;
				}
			}
		}
		static void ShowingMenu( bool showing )
		{
			if( !showing )
				DisablePartialMenusModeTemporarily = false;
			else
			{
				StopPartialMenusTimer();
			}
		}

		static MenuGrid()
		{
			// Don't need to call this in the beginning.
			//StartPartialMenusTimer();
		}
		static void TimerTick( object sender, EventArgs e )
		{
			disablePartialMenusModeTemporarily = false;
			StopPartialMenusTimer();
		}
		// Following this pattern to workaround issues in 1.1
		static void StartPartialMenusTimer()
		{
			if( ( timer != null ) || ( bInStopPartialMenusTimer == true ) )
				return;

			timer = new Timer();
			timer.Interval = BarManager.PartialMenusExpandedStateResetDelay;
			timer.Enabled = true;
			timer.Tick += new EventHandler( TimerTick );
		}
		static void StopPartialMenusTimer()
		{
			if( ( timer == null ) || ( bInStopPartialMenusTimer == true ) )
				return;

			bInStopPartialMenusTimer = true;

			timer.Stop();
			timer.Tick -= new EventHandler( TimerTick );
			timer.Dispose();
			timer = null;

			bInStopPartialMenusTimer = false;
		}
		#endregion TEMPORARY_STATE_MEMBERS
		#region PRIVATE_MEMBERS
		private MenuGridHost gridHost;
		private IPopupChild childMenuUI;
		private BarItem curDropDownItem;
		private IPopupParent parentMenuUI;
		private BarItem clickedItem = null;
		private Timer showTimer;
		private bool bInStopHideTimer = false;	// Workaround for .NET 1.1 Timer re-entry bug
		private bool bInStopShowTimer = false;	// Workaround for .NET 1.1 Timer re-entry bug
		private static bool bInStopPartialMenusTimer = false;	// Workaround for .NET 1.1 Timer re-entry bug
		private Timer hideTimer;
		internal int currentVisibleChildIndex = -1; // 0 based
		private ParentBarItem currentVisibleChildItem = null;
		private Point currentLocation = Point.Empty;
		private AnimationHelper animationHelper;
		internal bool partialMenusOn = false;
		private MouseProcHookerUtil mouseHooker = null;
		private KeyboardProcHooker keyboardHooker = null;
		private bool aboutToShow = false;
		private bool m_bSuppressIsClickingChange = false;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Current highlighted <see cref="BarItem"/>.
		/// </summary>
		private BarItem m_biHighlighted;
#endif
		#endregion PRIVATE_MEMBERS

		// Handler for property changes in Parent, Items collection and Children
		#region INITIALIZATION
		public MenuGrid()
			: base( true )
		{
			this.TopRowChanged += new GridRowColIndexChangedEventHandler( this.Grid_TopRowChanged );

			this.gridHost = new MenuGridHost();
			this.Parent = this.gridHost;

			this.animationHelper = new AnimationHelper();
			this.animationHelper.AnimationPositionChanged += new EventHandler( this.AnimationListener );
			this.animationHelper.AnimationDone += new EventHandler( this.AnimationDone_Event );

			this.listener = new MenuCloseEventListener( this );

			// Force creating the handle here for 2 reasons:
			// 1) Menu will appear faster the first time.
			// 2) In VerifyMenuStates, we check if the menu is still in a "valid" state
			//		by checking of the Handle is still available.
			IntPtr handle = this.Handle;
		}

		protected override internal void JitCode()
		{
			base.JitCode();
			this.EnsurePaintCodeJitted( true );
		}

		internal MenuGridHost Host
		{
			get { return this.gridHost; }
		}
		private void StartShowTimer()
		{
			if( this.showTimer != null || ( this.bInStopShowTimer == true ) )
				return;

			this.showTimer = new Timer();
			this.showTimer.Interval = 500;
			this.showTimer.Enabled = true;
			this.showTimer.Tick += new EventHandler( ShowTimerListener );
		}

		private void StopShowTimer()
		{
			if( ( this.showTimer == null ) || ( this.bInStopShowTimer == true ) )
				return;

			this.bInStopShowTimer = true;

			// Caching is to workaround issue in Final Beta of Everet.
			Timer cachedTimer = this.showTimer;
			this.showTimer = null;
			cachedTimer.Stop();
			cachedTimer.Tick -= new EventHandler( ShowTimerListener );
			cachedTimer.Dispose();
			cachedTimer = null;

			this.bInStopShowTimer = false;
		}

		private void StartHideTimer()
		{
			if( ( this.hideTimer != null ) || ( this.bInStopHideTimer == true ) )
				return;

			this.hideTimer = new Timer();
			this.hideTimer.Interval = 500;
			this.hideTimer.Enabled = true;
			this.hideTimer.Tick += new EventHandler( this.HideTimerListener );
		}

		private void StopHideTimer()
		{
			if( ( this.hideTimer == null ) || ( this.bInStopHideTimer == true ) )
				return;

			this.bInStopHideTimer = true;

			this.hideTimer.Stop();
			this.hideTimer.Tick -= new EventHandler( this.HideTimerListener );
			this.hideTimer.Dispose();
			this.hideTimer = null;

			this.bInStopHideTimer = false;
		}

		protected override void OnCurrentCellControlLostFocus( ControlEventArgs e )
		{
			base.OnCurrentCellControlLostFocus( e );
			// THis is necessary in cases where the user clicks outside the application.
			if( !this.Focused )
			{
				this.CurrentCell.ConfirmChanges();
				this.HidePopup(PopupCloseType.Deactivated);
			}
		}

		protected override void OnCurrentCellActivating( GridCurrentCellActivatingEventArgs e )
		{
			if( this.allowCurrentCellActivating )
				base.OnCurrentCellActivating( e );
			else
			{
				e.Cancel = true;
				return;
			}
		}

		// Will use my own host.
		public PopupHost PopupHost
		{
			get { return null; }
			set
			{
			}
		}

		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
			if( disposing )
			{
				if( this.gridHost != null )
				{
					gridHost.Dispose();
					gridHost = null;
				}
				this.TopRowChanged -= new GridRowColIndexChangedEventHandler( this.Grid_TopRowChanged );
				this.StopShowTimer();
				this.StopHideTimer();
				this.animationHelper.AnimationPositionChanged -= new EventHandler( this.AnimationListener );
				this.animationHelper.AnimationDone -= new EventHandler( this.AnimationDone_Event );
			}
			if( this.mouseHooker != null )
			{
				mouseHooker.Dispose();
				mouseHooker = null;
			}
			if( this.keyboardHooker != null )
			{
				keyboardHooker.Dispose();
				keyboardHooker = null;
			}
            this.listener.TargetControl = null;
            this.listener = null;
		}

		protected override void WndProc( ref Message m )
		{
			if( m.Msg == 0x21/*WM_MOUSEACTIVATE*/)
			{
				m.Result = (IntPtr)3;
				return;
			}

			base.WndProc( ref m );
		}
		private bool ProcessMouseMessage( Control destination, int msg )
		{
			if( msg == NativeMethods.WM_MOUSEMOVE || msg == NativeMethods.WM_MOUSELEAVE || msg == NativeMethods.WM_MOUSEHOVER
				|| msg == NativeMethods.WM_NCMOUSEMOVE || msg == NativeMethods.WM_NCMOUSELEAVE || msg == NativeMethods.WM_NCMOUSEHOVER
				|| msg == NativeMethods.WM_MOUSEWHEEL )
			{
				// When in hooks mode (non .net app), this gets called even when in drag and drop, 
				// so, not processing when mousebutton is pressed.
				if( Control.MouseButtons == MouseButtons.None )
				{
					// If the destination is in the parent chain, let it pass through
					if( ( destination != null && this.IsRelatedControl( destination, true ) )
						|| this.IsCurrentCellRelatedControl( destination )
						)
						return false;
					// else don't send it to the destination.
					else
						return true;
				}
			}
			else
				this.VeryifyMouseBasedDeactivation( msg, destination );

			return false;
		}

		public virtual bool MouseMessage( ref Message m )
		{
			Control destination = Control.FromHandle( m.HWnd );
			int msg = m.Msg;

			return this.ProcessMouseMessage( destination, msg );
		}
		protected virtual bool IsCurrentCellRelatedControl( Control control )
		{
			if( !this.CurrentCell.HasControlFocus )
				return false;

			GridCellRendererBase renderer = this.CurrentCell.Renderer;

			if( renderer is IFocusableRenderer )
				return ( (IFocusableRenderer)renderer ).IsRelatedControl( control );
			else
				return false;
		}

		private bool PostProcessKeyDown( int lParam, Keys key )
		{
			if( ( key == Keys.Menu || key == Keys.F10 )
				&& ( lParam & 0x40000000 ) == 0 )
			{
				if( this.parentItem.Manager != null && this.parentItem.Manager.MainFrameBarManager != null )
					this.parentItem.Manager.MainFrameBarManager.ignoreNextAltKeyUp = true;
				this.HidePopup( PopupCloseType.Deactivated );
				return true;
			}
			return false;
		}

		public virtual bool KeyboardMessage( ref Message m )
		{
			bool processed = false;

			if( this.CurrentCell.HasControlFocus )
				return false;

			// If key down:
			if( m.Msg == 0x0100 || m.Msg == 0x0102 || m.Msg == 0x0104 /*WM_SYSKEYDOWN*/)
			{
				Keys keys = (Keys)m.WParam.ToInt32();

				processed = this.ProcessKeyDown( keys );
			}

			if( !processed && !this.Customizing && m.Msg == 0x0100
				&& !( ( null != this.parentItem && this.parentItem.DesignMode ) || !this.DesignMode ) )
			{
				Keys key = (Keys)m.WParam.ToInt32();
				processed = this.PostProcessKeyDown( m.LParam.ToInt32(), key );
			}

			if( !processed )
				if( this.PopupParent is INeedKeyboardMessages )
					( (INeedKeyboardMessages)this.PopupParent ).KeyboardMessage( ref m );

			if( this.Customizing )
				return false;
			else
				return processed;
		}

		protected virtual void VeryifyMouseBasedDeactivation( int msg, Control destinationControl )
		{
			switch( msg )
			{
				case NativeMethods.WM_MOUSEACTIVATE: // 0x0021 
				case NativeMethods.WM_LBUTTONDOWN: // 0x0201 
				case NativeMethods.WM_RBUTTONDOWN: // 0x0204 
				case NativeMethods.WM_MBUTTONDOWN: // 0x0207 
				//if (this.IsRelatedControl(destinationControl, true))
				//{
				//    break;
				//}
				goto case NativeMethods.WM_NCLBUTTONDOWN;

				case NativeMethods.WM_NCLBUTTONDOWN: // 0x00a1
				case NativeMethods.WM_NCRBUTTONDOWN: // 0x00a4
				case NativeMethods.WM_NCMBUTTONDOWN: // 0x00a7
				{
					if( !this.IsRelatedControl( destinationControl, false ) )
					{
						// click outside 
						this.HidePopup( PopupCloseType.Deactivated );
					}
					break;
				}
			}
		}

		bool IKeyboardProcHookClient.KeyboardHookProc( int wParam, int lParam )
		{
			bool processed = false;

			if( this.CurrentCell.HasControlFocus )
				return false;

			// If key down:
			if( ( lParam & 0x80000000 ) == 0 )
			{
				Keys keys = (Keys)wParam;
				processed = this.ProcessKeyDown( keys );
			}

			if( this.parentItem != null && !this.parentItem.DesignMode )
			{
				if( !processed && !this.Customizing && ( lParam & 0x80000000 ) == 0/*key down*/)
				{
					Keys key = (Keys)wParam;
					processed = this.PostProcessKeyDown( lParam, key );
				}

				if( !processed )
					if( this.PopupParent is IKeyboardProcHookClient )
						( (IKeyboardProcHookClient)this.PopupParent ).KeyboardHookProc( wParam, lParam );

				if( this.Customizing )
					return false;
				else
					return true;
			}

			return processed;
		}

		bool IMouseHookHLProcClient.MouseHookProc( int msg, Point point, IntPtr hwnd, int wHitTestCode, int dwExtraInfo )
		{
			Control destinationControl = Control.FromHandle( hwnd );

			if( destinationControl != null || IntPtr.Zero != hwnd )
				return this.ProcessMouseMessage( destinationControl, msg );
			else
				return false;
		}

		protected override void UpdateStyleBasedOnItem( GridStyleInfo style, IRequiresControl barItem )
		{
			base.UpdateStyleBasedOnItem( style, barItem );

			ComboBoxBarItem comboBarItem = barItem as ComboBoxBarItem;

			if( comboBarItem != null )
			{
				style.CellType = comboBarItem.Editable ? "ComboBox" : "ComboBox_ListboxMode";
			}
		}

		private void AnimationDone_Event( object sender, EventArgs e )
		{
			if( !this.MenuBound )
				return;
			this.RefreshMenuGrid( true );
		}

		private void AnimationListener( object sender, EventArgs e )
		{
			if( !this.MenuBound )
			{
				this.animationHelper.StopAnimation();
				return;
			}

			// We will do the final processing in AnimationDone.
			if( this.animationHelper.AnimationPosition != this.animationHelper.MaxAnimationPosition )
			{
				BeginUpdate( BeginUpdateOptions.Invalidate );
				try
				{
					this.HighlightRange = GridRangeInfo.Empty;
					this.RefreshMenuGrid( false );
				}
				finally
				{
					EndUpdate();
				}
			}
		}

		protected virtual void ShowDropDown( bool selectDefaultItem )
		{
			this.StopShowTimer();
			this.StopHideTimer();

			GridRangeInfo range = this.HighlightRange;
			if( this.currentVisibleChildIndex == range.Top - 1 )
				return;
			else if( this.currentVisibleChildIndex != -1 )
				this.HideTimerListener( null, EventArgs.Empty );

			if( range == GridRangeInfo.Empty )
			{
				this.HideTimerListener( null, EventArgs.Empty );
				return;
			}

			// Show the child, if any
			BarItem barItem = this.parentItem.Items[range.Top - 1];
			if( !this.Customizing && !barItem.Enabled )
				return;

			if( barItem is ParentBarItem )
			{
				// TODO: Instead provide a method in ParentBarItem that can be queried before dropdown
				if( !( barItem is ToolbarListBarItem ) || !this.Customizing )
				{
					this.StopListeningForDeactivation();
					// You can directly invoke a MenuGrid for the children or do it indirectly via a Popup
					this.ShowChildrenUI( barItem as ParentBarItem, this, selectDefaultItem, null );
				}
			}
			else if( barItem is DropDownBarItem )
			{
				if( !this.Customizing )
				{
					DropDownBarItem item = barItem as DropDownBarItem;
					PopupControlContainer popupContainer = item.PopupControlContainer;

					// Need to check for DesignMode since this will be the case
					// while designing PopupMenus in the absence of a BarManager
					if( popupContainer != null && !popupContainer.DesignMode )
					{
						this.childMenuUI = popupContainer;
						this.curDropDownItem = barItem;

						this.StopListeningForDeactivation();

						popupContainer.PopupParent = this;
						popupContainer.RightToLeft = this.RightToLeft;
						popupContainer.ShowPopup( Point.Empty );
					}
				}
			}
			if( this.childMenuUI != null )
			{
				//				this.MouseHooker.HookMessages = false;
				//				this.KeyboardHooker.HookMessages = false;

				this.currentVisibleChildIndex = range.Top - 1;
				this.mouseDownPoint = this.PointToClient( Control.MousePosition );
			}
			else
				this.StartListeningForDeactivation();
		}

		private void ShowTimerListener( object sender, EventArgs e )
		{
			bool bPrevSuppress = this.SuppressIsClickingChange;
			this.SuppressIsClickingChange = true;

			this.ShowDropDown( false );

			this.SuppressIsClickingChange = bPrevSuppress;
		}

		protected internal bool IsCustomizing
		{
			get
			{
				bool bIsCustomizing = false;

				PopupMenu popupMenu = PopupParent as PopupMenu;
				if( popupMenu != null )
				{
					CommandBar commandBar = popupMenu.PopupParent as CommandBar;
					if( commandBar != null )
					{
						bIsCustomizing = commandBar.IsCustomizing;
					}
				}
				else
				{
					MenuGrid grid = PopupParent as MenuGrid;
					if( grid != null )
					{
						bIsCustomizing = grid.IsCustomizing;
					}
				}

				return bIsCustomizing;
			}
		}

		public virtual void ShowChildrenUI( ParentBarItem parent, IPopupParent parentUI,
			bool selectDefaultItem, Queue pbiQueue )
		{
			if( ( currentVisibleChildItem == parent ) && childMenuUI != null && childMenuUI.IsShowing() ) return;
			this.currentVisibleChildItem = parent;

			// The menuGrid will release itself based on settings
			MenuGrid menuGrid = XPMenuGridFactory.GetMenuGridToDeploy();

			this.childMenuUI = menuGrid;
			this.curDropDownItem = parent;

			parent.Style = parentItem.Style;
			//Show the menu

			menuGrid.IsClicking = this.IsClicking;
			menuGrid.SuppressIsClickingChange = this.SuppressIsClickingChange;

			menuGrid.Show( parent, Point.Empty, parentUI, selectDefaultItem, pbiQueue );
			if( !menuGrid.IsShowing() )
				this.ChildClosing( menuGrid, PopupCloseType.Canceled );
		}

		private void HideTimerListener( object sender, EventArgs e )
		{
			this.StopHideTimer();
			if( this.childMenuUI != null )
			{
				PopupControlContainer popup = this.childMenuUI as PopupControlContainer;
				if( popup == null || !( popup.IgnoreMouseMessages && m_bIsMouseMessage ) )
				{
					this.childMenuUI.HidePopup( PopupCloseType.Canceled );
				}

				ParentBarItem parentToShow = this.SelectedItem as ParentBarItem;
				if( parentToShow != null )
				{
					if( this.currentVisibleChildItem != parentToShow )
					{
						this.ShowDropDown( false );
					}
				}
			}

			this.currentVisibleChildIndex = -1;
		}

		#endregion INITIALIZATION

		#region PROPERTIES

		public bool IgnoreWorkingArea
		{
			get
			{
				return gridHost.IgnoreWorkingArea;
			}
			set
			{
				gridHost.IgnoreWorkingArea = value;
			}
		}

		private MouseProcHookerUtil MouseHooker
		{
			get
			{
				if( this.mouseHooker == null )
					this.mouseHooker = new MouseProcHookerUtil( Handle, this );
				return mouseHooker;
			}
			set
			{
				mouseHooker = value;
			}
		}
		internal KeyboardProcHooker KeyboardHooker
		{
			get
			{
				if( this.keyboardHooker == null )
					this.keyboardHooker = new KeyboardProcHooker( Handle, this );
				return keyboardHooker;
			}
			set
			{
				keyboardHooker = value;
			}
		}
		internal Point[] OverlapBorderCue
		{
			get
			{
				if( this.parentMenuUI != null )
					return this.parentMenuUI.GetBorderOverlapCue( this.gridHost.CurrentRAlign );
				else
					return null;
			}
		}

		bool IPopupParent.IsRightToLeft
		{
			get
			{
				return this.IsRightToLeft();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		internal protected bool IsRTL
		{
			get
			{
				return ( this as IPopupParent ).IsRightToLeft;
			}
		}

		protected override bool MenuBound
		{
			get { return base.MenuBound; }
			set
			{
				if( base.MenuBound != value )
				{
					ParentBarItem oldParentItem = this.parentItem;
					base.MenuBound = value;

					if( value == false && oldParentItem != null )
					{
						if( oldParentItem.Manager != null )
							oldParentItem.Manager.CustomizationDone
								-= new EventHandler( this.CustomizationDone );

						PopupManager.SetCurrentPopupClient( this, false, false );
					}

					this.clickedItem = null;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					m_biHighlighted = null;
#endif
					this.StopShowTimer();
					this.StopHideTimer();

					// Init certain members
					if( base.MenuBound )
					{
						ShowingMenu( true );
						if( this.parentItem.Manager != null )
						{
							this.parentItem.Manager.CustomizationDone
								+= new EventHandler( this.CustomizationDone );
						}
						PopupManager.SetCurrentPopupClient( this, true,
							false );

						this.StartListeningForDeactivation();
						//						if((this.parentItem.Manager != null && this.parentItem.Manager.useHooksForMenus)
						//							|| (this.parentItem.Manager == null && this.parentItem.DesignMode)
						//							// AllowQuit == false means this is hosted by a native windows app.
						//							|| (!this.parentItem.DesignMode && Application.AllowQuit == false)
						//							)
						//						{
						//							this.MouseHooker.HookMessages = true;
						//							this.KeyboardHooker.HookMessages = true;
						//						}
					}
					else
					{
						ShowingMenu( false );
						this.currentVisibleChildIndex = -1;
						this.currentVisibleChildItem = null;
						this.childMenuUI = null;
						this.curDropDownItem = null;

						this.StopListeningForDeactivation();
						//						this.MouseHooker.HookMessages = false;
						//						this.KeyboardHooker.HookMessages = false;
					}
				}
				// Helps while Jiting
				else
					base.MenuBound = value;

			}
		}
		protected virtual void StartListeningForDeactivation()
		{
			if( this.parentMenuUI != null )
				this.listener.TargetControl = this.parentMenuUI.GetPopupParentControl();
		}
		protected virtual void StopListeningForDeactivation()
		{
			this.listener.TargetControl = null;
		}

		private bool SuppressIsClickingChange
		{
			get
			{
				return m_bSuppressIsClickingChange;
			}
			set
			{
				m_bSuppressIsClickingChange = value;
			}
		}

		#endregion PROPERTIES

		#region DNDSTUFF
		protected virtual void CustomizationDone( object sender, EventArgs e )
		{
			if( this.parentItem != null && sender == this.parentItem.Manager && sender != null )
				this.HidePopup( PopupCloseType.Deactivated );
		}
		protected override void OnStartDragging()
		{
			this.CloseDropDowns();
		}

		protected override bool ShouldRemoveItemOnMove()
		{
			return true;
		}

		protected override void ResetDragging()
		{
			if( !this.MenuBound )
			{
				this.dndHelper.ParentItem = null;
				this.ReleaseGrid();
			}
			base.ResetDragging();
		}

		protected void CloseDropDowns()
		{
			if( this.childMenuUI != null )
				this.HideTimerListener( null, EventArgs.Empty );
		}
		#endregion DNDSTUFF
		#region MENU_OPERATIONS

		protected override void OnMouseEnter( EventArgs e )
		{
			base.OnMouseEnter( e );

			BarRenderer renderer = this.parentMenuUI as BarRenderer;

			if( renderer != null )
			{

				renderer.CancelHidingDropDown();
			}
		}


		protected override void OnPaintBackground( PaintEventArgs pevent )
		{
			bool customBackground = false;
			if( this.ParentItem != null )
			{
				Rectangle itemBounds = this.ClientRectangle;

				if( !this.IsRTL )
				{
					itemBounds.Offset( this.IconColumnWidth, 0 );
				}

				itemBounds.Width -= this.IconColumnWidth;
				customBackground =
					this.ParentItem.OnDrawBackground(
						new SubMenuPaintEventArgs( pevent, this.Bounds, itemBounds ) );
			}
			if( !customBackground )
				base.OnPaintBackground( pevent );
		}
		public bool CanScroll( bool down )
		{
			if( down )
			{
				return ViewLayout.HasPartialVisibleRows;
			}
			else
			{
				int newRow = TopRowIndex;
				newRow--;
				// Find a selectable row first at the top
				while( newRow > 0 && ( !Model[newRow, this.GetCol( 1 )].Enabled
					|| !this.IsSelectable( newRow )
					) )
					newRow--;

				if( newRow <= 0 || !this.IsSelectable( newRow ) )
					return false;
				else
					return true;
			}
		}
		public void Scroll( bool down )
		{
			bool scrolled = false;

			if( down )
			{
				int newRow = ViewLayout.LastVisibleRow;
				// If last row is not fully visible AND if the bottom row is not also the top row
				if( ViewLayout.HasPartialVisibleRows )
				{
					this.ScrollCellInView( newRow, this.GetCol( 1 ) );
					scrolled = true;
				}
				else
				{
					newRow++;
					while( newRow <= this.Model.RowCount && !this.IsSelectable( newRow ) )
						newRow++;

					if( newRow <= this.Model.RowCount && this.IsSelectable( newRow ) )
					{
						this.ScrollCellInView( newRow, this.GetCol( 1 ) );
						scrolled = true;
					}
				}
			}
			else
			{
				int newRow = TopRowIndex;
				newRow--;
				// Find a selectable row first at the top
				while( newRow > 0 && !this.IsSelectable( newRow ) )
					newRow--;

				if( newRow > 0 && this.IsSelectable( newRow ) )
				{
					this.ScrollCellInView( newRow, this.GetCol( 1 ) );
					scrolled = true;
				}
			}
			if( scrolled )
				this.gridHost.UpdateScrollStates( false );
		}

		protected bool IsCurrentCellActivatable()
		{
			if( !this.CurrentCell.HasCurrentCell )
				return false;

			int rowIndex = CurrentCell.RowIndex;

			if( this.parentItem != null && rowIndex > 0 && rowIndex <= this.parentItem.Items.Count
				&& ( this.parentItem.Items[rowIndex - 1] is IRequiresControl ) )
				return true;

			return false;
		}

		private bool m_bIsMouseMessage = false;

		private bool m_bIsMouseDownProcessing = false;

		internal bool IsMouseDownProcessing
		{
			get
			{
				return m_bIsMouseDownProcessing;
			}
		}

		internal bool IsMouseMessage
		{
			get
			{
				return m_bIsMouseMessage;
			}
			set
			{
				if( value != m_bIsMouseMessage )
				{
					m_bIsMouseMessage = value;
				}
			}
		}
		protected override void OnMouseUp( MouseEventArgs e )
		{
			m_bIsMouseMessage = true;

			// OnMouseUp could be called after this control gets disposed, if it is a right
			// mouse up that follows a context menu popup (in whose handler the control could have been
			// destroyed).
			if( !this.IsHandleCreated )
				return;

			if( this.animationHelper.AnimationOn )
				return;

			base.OnMouseUp( e );

			if( e.Button != MouseButtons.Left )
				return;

			if( this.IsCurrentCellActivatable() )
				return;

			if( this.SelectedIndex != -1 )
			{
				FreeObjectCache();
				this.RefreshRange( this.HighlightRange );
			}

			if( !this.Customizing )
			{
				if( this.HighlightRange != GridRangeInfo.Empty && m_bShouldPerformClick )
				{
					BarItem clicked = this.parentItem.Items[this.HighlightRange.Top - 1];
					bool isDropDownStyle = this.IsItemDropDownStyle( clicked );
					if( clicked is ParentBarItem && !isDropDownStyle
						|| !this.IsSelectedItemClickableAt( new Point( e.X, e.Y ) )
						)
					{
						if( clicked is DropDownBarItem )
						{
							ShowDropDown( false );
						}

                        return;
					}
					
					this.ProcessItemClick( clicked );
				}
			}

			m_bIsMouseMessage = false;
			this.IsClicking = false;
		}

		protected virtual void OnRightMouseDown( MouseEventArgs e )
		{
			if( this.Customizing )
			{
				// Fake a left mouse click, so that selected item will be updated
				MouseEventArgs args = new MouseEventArgs( MouseButtons.Left, e.Clicks, e.X, e.Y, e.Delta );
				this.OnMouseDown( args );

				this.mouseDownPoint = this.PointToClient( Control.MousePosition );
				if( this.childMenuUI != null )
					// This MouseDown should actually hide the open child, if any
					this.HideTimerListener( null, EventArgs.Empty );

				// Show the customization popup
				if( this.SelectedIndex != -1 )
				{
					// Cancel the existing popups
					while( PopupManager.ActivePopupClient != this
						&& PopupManager.ActivePopupClient != null )
					{
						PopupManager.ActivePopupClient.HidePopup( PopupCloseType.Canceled );
					}

					// To fake my close-up
					PopupManager.SetCurrentPopupClient( this, false, false );

					this.ValidateDndHelper();

					this.CustomizationPopup.Show( this, new Point( e.X, e.Y ),
						this.SelectedItem, this.parentItem, this.dndHelper );
				}
			}
		}

		private bool m_bIsClick = false;
		private bool m_bShouldPerformClick = false;

		internal Point m_clickedMousePos = Point.Empty;

		internal bool IsClicking
		{
			get
			{
				return m_bIsClick;
			}
			set
			{
				BarRenderer barRenderer = parentMenuUI as BarRenderer;

				if( !this.SuppressIsClickingChange && !( null != barRenderer && barRenderer.SuppressIsMenuClickingChange ) )
				{
					if( value )
					{
						m_clickedMousePos = Control.MousePosition;
					}
					else
					{
						m_bShouldPerformClick = false;
						m_clickedMousePos = Point.Empty;
					}

					m_bIsClick = value;
				}
			}
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			m_bShouldPerformClick = true;
			m_bIsMouseMessage = true;
			m_bIsMouseDownProcessing = true;

			if( this.animationHelper.AnimationOn )
				return;

			bool customizing = this.Customizing;

			if( this.CurrentCell.HasCurrentCell )
			{
				GridRangeInfo range = this.PointToRangeInfo( new Point( e.X, e.Y ), -1 );
				if( this.CurrentCell.RowIndex != range.Top )
				{
					this.allowCurrentCellActivating = true;
					this.CurrentCell.MoveTo( GridRangeInfo.Empty );
					this.allowCurrentCellActivating = false;
				}
			}

			GridRangeInfo oldRange = this.HighlightRange;

			if( e.Button == MouseButtons.Right )
			{
				this.OnRightMouseDown( e );

				// Search for "Need WantKeys" in this file
				this.WantKeys = false;

				base.OnMouseDown( e );

				this.WantKeys = true;
				return;
			}

			BeginUpdate( BeginUpdateOptions.Invalidate );
			try
			{
				// Search for "Need WantKeys" in this file
				this.WantKeys = false;

				base.OnMouseDown( e );

				this.WantKeys = true;

				if( e.Button == MouseButtons.Left
					&& oldRange == this.HighlightRange )
				{
					BarItem selectedItem = this.SelectedItem;
					bool isDropDownStyle = this.IsItemDropDownStyle( selectedItem );
					bool justClosed = false;

					if( this.childMenuUI != null )
					{
						if( customizing
							|| ( !customizing && this.curDropDownItem != selectedItem )
							|| ( isDropDownStyle && this.IsSelectedItemClickableAt( new Point( e.X, e.Y ) ) ) )
						{
							if( this.curDropDownItem == selectedItem )
								justClosed = true;
							this.mouseDownPoint = new Point( e.X, e.Y );
							// This MouseDown should actually hide the open child, if any
							this.HideTimerListener( null, EventArgs.Empty );
						}
					}

					if( !justClosed && this.childMenuUI == null )
					{
						// Open the child if clicked on the arrow or just a plain ParentBarItem
						if( customizing
							|| ( this.SelectedItem is ParentBarItem && !isDropDownStyle )
							|| ( isDropDownStyle && !this.IsSelectedItemClickableAt( new Point( e.X, e.Y ) ) ) )
						{
							this.mouseDownPoint = new Point( e.X, e.Y );
							this.ShowDropDown( false );
						}
						else
						{
							this.StopShowTimer();
							this.IsClicking = true;
						}
					}
					if( !customizing && this.SelectedItem is IRequiresControl )
					{
						int col = ( selectedItem.Text.Length > 0 ) ? ShortcutColumn : TextColumn;

						this.listener.ignoreDeactivate = true;
						this.gridHost.Activate();
						this.Focus();

						this.allowCurrentCellActivating = true;
						this.CurrentCell.MoveTo( this.HighlightRange.Top, col, GridSetCurrentCellOptions.SetFocus );
						this.allowCurrentCellActivating = false;

						this.listener.ignoreDeactivate = false;
					}
				}
				if( !this.CurrentCell.HasCurrentCell )
				{
					FreeObjectCache();
					this.InvalidateRange( this.HighlightRange );
				}
			}
			finally
			{
				EndUpdate();
			}
            if (this.CurrentCell.Renderer != null)
            {
                if(this.CurrentCell.Renderer is MenuComboBoxCellRenderer)
                (this.CurrentCell.Renderer as MenuComboBoxCellRenderer).TextBox.SelectAll();
            }
			m_bIsMouseMessage = false;
			m_bIsMouseDownProcessing = false;
		}

		private Form GetCustomizationDialog()
		{
			if( !this.MenuBound )
				return null;

			if( this.Customizing )
				return this.parentItem.Manager.CustomizationDialog;

			return null;
		}

		public virtual bool IsRelatedControl( Control control, bool askParent )
		{
			if( !this.MenuBound )
				return false;

			if( control == this || this.Contains( control )
				|| control == this.gridHost || this.gridHost.Contains( control )
				|| ( this.IsCustomizationPopupAvailable && this.CustomizationPopup.IsPopupUI( control ) )
				|| control == this.parentMenuUI )
			{
				return true;
			}

			BarControlInternal bci = control as BarControlInternal;

			if( bci != null )
			{
				BarRenderer br = this.parentMenuUI as BarRenderer;

				if( bci.barRenderer == br && ( br.CurrentHotTrackItem == this.parentItem ) )
				{
					return true;
				}
			}

			// Check, if it belongs to the customization dlg, then 
			Form cusomizationDlg = this.GetCustomizationDialog();

			if( cusomizationDlg != null
				&& ( cusomizationDlg == control || cusomizationDlg.Contains( control ) ) )
			{
				return true;
			}
			else
			{
				MenuGrid menuGrid = control as MenuGrid;

				if( menuGrid != null )
				{
					ParentBarItem pbi = menuGrid.parentItem;

					if( pbi != null && pbi.HasChild( this.parentItem ) )
					{
						askParent = pbi.CloseOnClick;
					}
				}

				return askParent && this.parentMenuUI.IsRelatedControl( control, askParent );
			}
		}

		protected internal virtual void ProcessItemClick( BarItem item )
		{
			this.clickedItem = item;
            PopupMenu popupMenu = PopupParent as PopupMenu;
            bool commandBarDropDown = false;
            if (popupMenu != null)
            {
                CommandBar commandBar = popupMenu.PopupParent as CommandBar;
                if (commandBar != null && !(item is ButtonVisibilityToggleBarItem))
                {
                    commandBarDropDown = true;
                }
            }
            
			if((( item == null || ( null != this.parentItem && ( this.parentItem.CloseOnClick ||commandBarDropDown) ) )))
			{
				this.clickedItem = item;
				this.HidePopup( PopupCloseType.Done );
			}
			else if( item != null )
			{
				this.mouseDownPoint = this.PointToClient( Control.MousePosition );
				this.clickedItem = null;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				m_biHighlighted = null;
#endif
				this.NotifyItem( item );
			}
		}

		private bool IsSelectedItemClickableAt( Point ptClick )
		{
			if( this.HighlightRange != GridRangeInfo.Empty )
			{
				BarItem itemClicked = this.parentItem.Items[this.HighlightRange.Top - 1] as BarItem;
				if( itemClicked.Enabled
					&& !( itemClicked is IRequiresControl )
					&& !( itemClicked is ParentBarItem )
					&& !( itemClicked is DropDownBarItem ) )
					return true;

				if( this.IsItemDropDownStyle( itemClicked ) )
				{
					GridRangeInfo range = this.PointToRangeInfo( ptClick, -1 );
					int col = range.Left;

					if( col != MenuGridControlBase.ArrowColumn )
						return true;
				}
				if( itemClicked is IRequiresControl )
				{
					return false;
				}
			}
			return false;
		}
		// Call PerformClick if there is a selected item and return true.
		protected virtual void NotifyItem( BarItem item )
		{
			if( item != null )
			{
				if( item.Enabled )
				{
					// So that all captures are released. This is necessary in case the Click handler begins a Message Loop
					this.Capture = true;
					this.Capture = false;
					item.PerformClick();
				}
			}
		}

		public bool NeedDelayedExpansion()
		{
			if( this.parentItem == null )
				return false;
			else if( this.parentItem.Manager != null )
				return this.parentItem.Manager.ExpandPartialMenusAfterDelay;
			else
				return true;
		}

		public void ExpandPartialMenus( bool forceOpen )
		{
			if( partialMenusOn && !this.CurrentCell.HasControlFocus
				&& !this.CurrentCell.IsDroppedDown
				&& ( forceOpen || this.childMenuUI == null ) )
			{
				this.partialMenusOn = false;
				DisablePartialMenusModeTemporarily = true;
				this.animationHelper.StartAnimation( 3, true, 10 );
			}
		}

		internal override void Show( ParentBarItem parentItem, Point location,
			IPopupParent parentUI, bool setDefaultSelection, Queue pbiQueue )
		{
			CancelEventArgs args = new CancelEventArgs( false );
			parentItem.OnBeforePopup( args );
			if( args.Cancel )
			{
				return;
			}

			// Need about to show in case the parent queries my visibility via IsShowing in WM_MOUSELEAVE
			// Also lets the user know whether this ParentBarItem is being shown via the menus or the PopupMenus (in the Popup handler).
			this.aboutToShow = true;

			parentItem.OnPopup( EventArgs.Empty );

			if( !( parentItem is ToolbarListBarItem ) )
			{
				BarRenderer renderer = parentUI as BarRenderer;
				if( renderer != null && !renderer.Customizing &&
					( parentItem.Items.Count == 0 ||
					( parentItem.Items.Count == 1 && parentItem.Items[0] is StandAloneBarItem ) ) )
				{
					this.aboutToShow = false;
					return;
				}
			}

			this.parentMenuUI = parentUI;
			this.IsClicking = false;

			this.ScrollOnMouseMove = parentItem.ScrollOnMouseMove;

			// There seems to be a bug in Control imp. that fails to consider the case
			// where a new window is opened when the mouse is over an old window.
			// The old window then doesn't seem to get subsequent WM_MOUSELEAVEs.
			// Manually sending a WM_MOUSELEAVE seems to help.
			Point ptMouse = Control.MousePosition;
			IntPtr windowAtPoint = NativeMethods.WindowFromPoint( ptMouse.X, ptMouse.Y );
			NativeMethods.SendMessage( windowAtPoint, NativeMethods.WM_MOUSELEAVE, IntPtr.Zero, IntPtr.Zero );
			this.aboutToShow = false;

			this.currentLocation = location;

			// To release anyone else's Capture.
			this.Capture = true;
			this.Capture = false;

			if( parentItem.Manager == null || !parentItem.Manager.Customizing )
				this.partialMenusOn = parentItem.UsePartialMenus && !DisablePartialMenusModeTemporarily
					&& this.NeedPartialMenus( parentItem );
			else
				this.partialMenusOn = false;

			this.RightToLeft = parentUI.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
			base.Show( parentItem, Point.Empty, parentUI, setDefaultSelection, null );

			if( setDefaultSelection )
				this.MoveSelection( MoveHint.moveFirst );

			if( null != pbiQueue && pbiQueue.Count > 0 )
			{
				ParentBarItem pbi = pbiQueue.Dequeue() as ParentBarItem;

				SetSelectionAtRow( parentItem.Items.IndexOf( pbi ) + 1 );

				this.parentItem = parentItem;
				this.ShowChildrenUI( pbi, this, setDefaultSelection, pbiQueue );
			}
		}

		private bool NeedPartialMenus( ParentBarItem parentItem )
		{
			if( parentItem.Manager != null
				&& !parentItem.Manager.UsePartialMenus )
				return false;

			if( parentItem.Manager != null && parentItem.Manager.MainFrameBarManager != null
				&& !parentItem.Manager.MainFrameBarManager.UsePartialMenus )
				return false;

			// Check if atleast 1 of the not-recently-used item is Visible.
			foreach( BarItem item in parentItem.Items )
			{
				if( !item.IsRecentlyUsedItem && ShouldDrawVisible( this.parentItem, item, this.Customizing ) )
					return true;
			}
			return false;
		}

		public Point GetLocationForPopupAlignment( PopupRelativeAlignment prevAlign,
			out PopupRelativeAlignment newAlign )
		{
			if( this.childMenuUI == null )
			{
				newAlign = PopupRelativeAlignment.Default;
				return Point.Empty;
			}

			// Assuming the hightlight range is in sync with the current child
			Rectangle selBounds = this.RangeInfoToRectangle( this.HighlightRange, GridRangeOptions.None );

			selBounds.X = 0;
			selBounds.Width = this.Width;

			if( this.IsRTL )
			{
                selBounds.X = 1;
				prevAlign = PopupRelativeAlignment.RightBottom;
			}

			Point childLoc = PopupUtils.ComputeDefaultLeftRightAlignment( prevAlign, out newAlign, selBounds, false );

			return this.PointToScreen( childLoc );
		}

		public Point GetPreferredLocation( PopupRelativeAlignment prevAlign,
			out PopupRelativeAlignment newAlign )
		{
			if( this.parentMenuUI != null )
			{
				Point pt = Point.Empty;

				BarControlBarRenderer renderer = this.parentMenuUI as BarControlBarRenderer;
				if( renderer != null )
				{
					bool bIsRightToLeft = ( RightToLeft == RightToLeft.Yes );

					pt = renderer.GetLocationForPopupAlignment( prevAlign,
						out newAlign, bIsRightToLeft );

					if(bIsRightToLeft)
					{
						switch (newAlign)
						{
							case PopupRelativeAlignment.RightBottom:
								pt.X -= (int)gridHost.Width;
								break;
							case PopupRelativeAlignment.TopRight:
								pt.X += (int)gridHost.Width;
								break;
						}
					}
				}
				else
				{
					pt = parentMenuUI.GetLocationForPopupAlignment( prevAlign, out newAlign );
				}

				if( !( newAlign == PopupRelativeAlignment.Default
					&& pt == Point.Empty ) )
					return pt;
			}

			newAlign = PopupRelativeAlignment.Default;

			if( !this.currentLocation.IsEmpty )
			{
				return this.currentLocation;
			}

			return new Point( 1, 1 );
		}

		public int PreferredHeight
		{
			get
			{
				return this.Model.RowHeights.GetTotal( 0, DisplayedItemsCount );
			}
		}

		public int DisplayedItemsCount
		{
			get
			{
				return Math.Min( MaximumItemsToDisplay, this.Model.RowCount );
			}
		}

		public int MaximumItemsToDisplay
		{
			get
			{
				return ( parentItem == null ) ? int.MaxValue :
					parentItem.MaximumItemsToDisplay;
			}
		}

		public bool ShowScrollBars
		{
			get
			{
				return ( this.MaximumItemsToDisplay < this.Model.RowCount );
			}
		}

		protected override void RefreshMenuGrid( bool bNoShow )
		{
			this.FreeObjectCache();

			Model.BeginUpdate( BeginUpdateOptions.None );

			this.HighlightRange = GridRangeInfo.Empty;

			this.ResizeRowHeights();

			if( !this.animationHelper.AnimationOn )
				this.ResizeColumns();

			// Set the size of the parent to be the size of the grid itself.
			int width = this.Model.ColWidths.GetTotal( 0, Model.ColCount );
			int height = this.Model.RowHeights.GetTotal( 0, DisplayedItemsCount );

			Size prefSize = new Size( width, height );

			this.gridHost.RightToLeft = this.RightToLeft;

			if( bNoShow )
			{
				this.gridHost.RefreshLayout( this, prefSize, this.partialMenusOn );
			}
			else
			{
				this.gridHost.ShowMenu( this, prefSize, this.partialMenusOn );
			}

			Model.EndUpdate( false );
			this.Refresh();
		}
		#endregion MENU_OPERATIONS
		#region UI_EVENTS

		protected override void ModelQueryRowHeight( object sender, GridRowColSizeEventArgs e )
		{
			base.ModelQueryRowHeight( sender, e );

			int rowIndex = e.Index;
			int rowHeight = e.Size;
			if( ( rowIndex - 1 ) >= 0 && this.MenuBound
				&& rowIndex <= this.parentItem.Items.Count )
			{
				BarItem barItem = this.parentItem.Items[rowIndex - 1];
				if( !barItem.IsRecentlyUsedItem && !this.Customizing )
				{
					if( this.partialMenusOn )
					{
						e.Handled = true;
						e.Size = 0;
					}
					else if( this.animationHelper.AnimationOn )
					{
						// An intermediate value
						int height = rowHeight / 2; // atleast half the height to start with.
						rowHeight = rowHeight - height;
						height += (int)Math.Ceiling( ( (float)rowHeight * ( ( (float)this.animationHelper.AnimationPosition / (float)this.animationHelper.MaxAnimationPosition ) ) ) );
						e.Size = height;
						e.Handled = true;
					}
				}
			}
		}
		protected override void OnPaint( PaintEventArgs pevent )
		{
			base.OnPaint( pevent );

			if( !this.MenuBound )
				return;

			Graphics g = pevent.Graphics;
			if( this.Customizing && this.parentItem.Manager != null )
			{
				int customizingItemIndex = -1;

				if( this.cachedSelectedItem != null )
					customizingItemIndex = this.parentItem.Items.IndexOf(
						this.cachedSelectedItem );
				else
					customizingItemIndex = this.parentItem.Items.IndexOf(
					this.parentItem.Manager.CustomizingItem );

				if( customizingItemIndex != -1 )
				{
					Rectangle customizingCellBounds =
						this.RangeInfoToRectangle( GridRangeInfo.Row( customizingItemIndex + 1 ), GridRangeOptions.None );

					customizingCellBounds.Offset( 1, 1 );
					customizingCellBounds.Width -= 2;
					customizingCellBounds.Height -= 2;
					Pen pen = new Pen( SystemColors.ControlText, 2 );
					g.DrawRectangle( pen, customizingCellBounds );
					pen.Dispose();
				}
			}
		}

		// Set selection to the row under mouse
		protected override void OnMouseMove( MouseEventArgs e )
		{
			m_bIsMouseMessage = true;

			if( m_bIsClick && ( Math.Abs( Control.MousePosition.X - m_clickedMousePos.X ) >=
				SystemInformation.DragSize.Width ) && ( Math.Abs( Control.MousePosition.Y -
				m_clickedMousePos.Y ) >= SystemInformation.DragSize.Height ) )
			{
				m_bShouldPerformClick = true;
			}

			if( this.animationHelper.AnimationOn || this.CurrentCell.HasCurrentCell )
				return;

			if( this.mouseDownPoint != Point.Empty && this.mouseDownPoint == new Point( e.X, e.Y ) )
				return;

			base.OnMouseMove( e );
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			BarItem barItem = null;
#endif
			if( !this.Customizing )
			{
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
				this.SetSelectionAt( new Point( e.X, e.Y ) );
#else
				barItem = this.SetSelectionAt( new Point( e.X, e.Y ) );

				if( null != barItem && m_biHighlighted != barItem )
				{
					SuperToolTip.SetToolTips( this, barItem );
					if (barItem is ParentBarItem)
						barItem.ShowToolTipInPopUp = (barItem as ParentBarItem).ShowToolTipInPopUp;
					toolTip.SetToolTip(this, barItem.Tooltip);
					if (barItem.ShowToolTipInPopUp)
					toolTip.ShowAlways = true;
					else
					toolTip.ShowAlways = false;
				}
#endif
			}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			m_biHighlighted = barItem;
#endif
			m_bIsMouseMessage = false;
		}

		protected override void OnMouseLeave( EventArgs e )
		{
			m_bIsMouseMessage = true;

			this.toolTip.ShowAlways = false;
			if( this.animationHelper.AnimationOn )
				return;

			base.OnMouseLeave( e );
			// Hide selection
			if( !this.Customizing && !this.CurrentCell.HasCurrentCell )
			{
				if( this.childMenuUI != null )
					UpdateHighlight();
				else
					this.HighlightRange = GridRangeInfo.Empty;
			}

			m_bIsMouseMessage = false;
		}

		// Update the hightlight if mouse over drop-down child
		protected virtual void UpdateHighlight()
		{
			if( this.currentVisibleChildIndex == -1 )
				return;

			IntPtr wndFromPoint = NativeMethods.WindowFromPoint( Control.MousePosition.X, Control.MousePosition.Y );
			if( wndFromPoint != IntPtr.Zero )
			{
				Control controlUnderMouse = Control.FromHandle( wndFromPoint );
				if( controlUnderMouse is IPopupChild )
				{
					IPopupChild popupChild = controlUnderMouse as IPopupChild;
					if( popupChild.IsRelatedControl( this, true ) )
						this.HighlightRange = GridRangeInfo.Row( this.currentVisibleChildIndex + 1 );
				}
                else
                {
                    this.HighlightRange = GridRangeInfo.Row(this.currentVisibleChildIndex + 1);
                }
			}
		}

		List<BarItem> samekeybar = new List<BarItem>();
		char lastkeychar=' ';
		bool first = false;
		int samekeycount = 0;
		BarItem item = null;
		protected override bool ProcessMnemonic( char charCode )
		{
			bool select = true;
			if (lastkeychar != charCode)
			{
				lastkeychar = charCode;
				first = true;
				samekeycount = 0;
				samekeybar.Clear();
			}
			else
			{
				first = false;
			}

			if( this.Customizing || ( Control.ModifierKeys != Keys.None && Control.ModifierKeys != Keys.Alt ) )
				return false;

			if( this.MenuBound && this.childMenuUI == null && !this.Customizing )
			{
                if (first)
                {
                    foreach (BarItem tbaritem in this.parentItem.Items.sameCharHotKey.Values)
                    {
                        int shrotcut = tbaritem.Text.IndexOf("&");
                        shrotcut++;
                        char temp = Char.ToLower(tbaritem.Text.ToCharArray()[shrotcut]);
                        if (temp == Char.ToLower(charCode))
                        {
                            samekeybar.Add(tbaritem);
                            samekeycount++;
                        }
                    }
                }
                
                if (samekeycount > 1)
                {
                    if (first)
                    {
                        item = samekeybar[0]; 
                    }
                    else
                    {
                        int i = samekeybar.IndexOf(item);
                        i++;
                        if (i < samekeybar.Count)
                            item = samekeybar[i];
                        else
                            item = samekeybar[0];
                    }
                    string baritemtext = this.Model[this.HighlightRange.Top, 2].CellValue.ToString();
                    if (item.Text == baritemtext)
                    {
                        BarItem it = this.parentItem.Items[this.HighlightRange.Top - 1]; 
                        int i = samekeybar.IndexOf(it);
                        i++;
                        if (i < samekeybar.Count)
                            item = samekeybar[i];
                        else
                            item = samekeybar[0];
                    }
                    select = false;
                }
                else
                {
                    item = this.parentItem.Items.GetItemFromHotKey(Char.ToLower(charCode));
                }
				if( item != null && MenuGridControlBase.ShouldDrawVisible( this.parentItem, item, this.Customizing ) )
				{
					this.HighlightRange = GridRangeInfo.Row( this.parentItem.Items.IndexOf( item ) + 1 );
					this.ShowDropDown( true );

					// If not a DropDown item, PerformClick and go back to the old item
                    if( !( item is DropDownBarItem || item is ParentBarItem ) && select)
					{
						this.ProcessItemClick( item );
					}

					return true;
				}
				else
				{
					bool handled = false;

					MenuGrid grid = this.parentMenuUI as MenuGrid;
					if( grid != null )
					{
						handled = grid.ProcessMnemonic( charCode );
					}

					if( !handled )
					{
						BarRenderer renderer = this.parentMenuUI as BarRenderer;
						if( renderer != null )
						{
							handled = renderer.ProcessShortcut( charCode );
						}
					}

					return handled;
				}
			}
			return base.ProcessMnemonic( charCode );
		}

		private bool ShowChildDropDown()
		{
			IPopupChild oldChildMenuUI = this.childMenuUI;
			this.ShowDropDown( true );

			return ( this.childMenuUI != null && oldChildMenuUI != this.childMenuUI );
		}

		private bool ClosePopup()
		{
			bool bResult = this.parentMenuUI != null && this.parentMenuUI is MenuGrid;

			if( bResult )
			{
				this.HidePopup( PopupCloseType.Canceled );
			}

			return bResult;
		}

		private bool m_bIsEscapeKeyPressed = false;


		internal bool IsEscapeKeyPressed
		{
			get
			{
				return m_bIsEscapeKeyPressed;
			}
		}

		protected override bool ProcessKeyDown( Keys keyCode )
		{
			if( this.Customizing || this.animationHelper.AnimationOn || !this.IsShowing())
				return false;

			BarItem item = null;

			if (this.HighlightRange != GridRangeInfo.Empty)
				item = this.parentItem.Items[this.HighlightRange.Top - 1];

			bool bControl = (Control.ModifierKeys & Keys.Control) != Keys.None;

			if (item is ComboBoxBarItem || item is TextBoxBarItem)
			{
					IntPtr hwnd = NativeMethods.GetFocus();
					GridDropDownEditPartControl isDropDownTextBox = Control.FromHandle(hwnd) as GridDropDownEditPartControl;
					if (isDropDownTextBox != null)
					{
						if (keyCode == Keys.Escape || keyCode == Keys.Tab)
						{
							isDropDownTextBox.ParentCell.CurrentCell.EndEdit();
						}
						if (bControl && keyCode == Keys.Z)
							isDropDownTextBox.ParentCell.CurrentCell.Grid.Model.CommandStack.Undo();
						if (bControl && keyCode == Keys.Y)
							isDropDownTextBox.ParentCell.CurrentCell.Grid.Model.CommandStack.Redo();
					}
                    //if (keyCode == Keys.Return)
                    //{
                    //    //MS behavior no change occurs
                    //    return true;
                    //}
			}

			bool handled = base.ProcessKeyDown( keyCode );

			if( this.CurrentCell.HasControlFocus )
				handled |= ( keyCode != Keys.Escape );

			if( !handled )
			{
				switch( keyCode )
				{
					case Keys.Right:
					{
						if( this.IsRTL )
						{
							if( ClosePopup() ) handled = true;
						}
						else
						{
							handled = ShowChildDropDown();
						}
						break;
					}
					case Keys.Left:
					{
						if( this.IsRTL )
						{
							handled = ShowChildDropDown();
						}
						else
						{
							if( ClosePopup() ) handled = true;
						}
						break;
					}
					// Hide if Escape key was pressed
					case Keys.Escape:
					{
						m_bIsEscapeKeyPressed = true;
						this.HidePopup( PopupCloseType.Canceled );
						m_bIsEscapeKeyPressed = false;
						handled = true;
						break;
					}
					case Keys.Return:
					{
						// Try to open the child if any
						handled = ShowChildDropDown();

						// If opening child was unsuccessful
						if( !handled && this.childMenuUI == null )
						{
							if( this.HighlightRange != GridRangeInfo.Empty )
								this.clickedItem = this.parentItem.Items[this.HighlightRange.Top - 1];

							handled = ( ( clickedItem != null ) && IsShowing() );
							this.ProcessItemClick( this.clickedItem );
						}
						break;
					}
				}
			}

			bool bIsShowing = IsShowing();

			if( !handled && this.parentItem != null && bIsShowing )
			{
				Keys keyData = keyCode;

				bool bCtrl = NativeMethods.GetAsyncKeyState( Keys.ControlKey ) > 0;
				bool bShift = NativeMethods.GetAsyncKeyState( Keys.ShiftKey ) > 0;
				bool bAlt = NativeMethods.GetAsyncKeyState( Keys.Menu ) > 0;

				keyData = bCtrl ? keyData | Keys.Control : keyData;
				keyData = bShift ? keyData | Keys.Shift : keyData;
				keyData = bAlt ? keyData | Keys.Alt : keyData;

				handled = this.parentItem.ProcessShortcut( keyData );
				if( handled )
				{
					HidePopup( PopupCloseType.Done );
				}
			}

			if( !handled )
			{
				if( ( keyCode >= Keys.D0 && keyCode <= Keys.Z ) )
				{
					this.ProcessMnemonic( (char)(int)keyCode );
					handled = bIsShowing;
				}
				else if( keyCode >= Keys.NumPad0 && keyCode <= Keys.NumPad9 )
				{
					this.ProcessMnemonic( (char)( 48 + ( keyCode - Keys.NumPad0 ) ) );
					handled = bIsShowing;
				}
			}

			if( !handled && bIsShowing )
			{
				if( ( ( keyCode & Keys.Oem8 ) != Keys.None ) && keyCode != Keys.Down &&
					keyCode != Keys.Up && keyCode != Keys.Left && keyCode != Keys.Right &&
					keyCode < Keys.F1 && keyCode > Keys.F24 )
				{
					handled = true;
				}
				else if( keyCode == Keys.Space )
				{
					handled = true;
				}
			}
			return handled;
		}

		protected override void HideHighlight( GridRangeInfo range )
		{
			base.HideHighlight( range );

			//this.StopHideTimer();

			if( this.currentVisibleChildIndex != -1 || ( this.childMenuUI != null && this.childMenuUI.IsShowing() ) )
			{
				if( this.Customizing )
					// Hide immediately
					this.HideTimerListener( null, EventArgs.Empty );
				else
				{
					this.StartHideTimer();
				}
			}
		}
		protected override void ShowHighlight( GridRangeInfo range )
		{
			base.ShowHighlight( range );

			this.StopShowTimer();

			if( this.currentVisibleChildIndex == range.Top - 1 )
			{
				this.StopHideTimer();
			}
			else
			{
				if( this.Customizing )
					// Show immediately
					this.ShowDropDown( false );
				else
				{
					if( !this.dontShowChild )
						this.StartShowTimer();
				}
			}
		}
		private void Grid_TopRowChanged( object sender, GridRowColIndexChangedEventArgs e )
		{
			this.gridHost.UpdateScrollStates( false );
		}
		#endregion UI_EVENTS

		#region GRID_VIRTUAL_OVERRIDES
		protected override void OnDrawCell( GridDrawCellEventArgs e )
		{
		}
		protected override bool MoveSelection( MoveHint hint )
		{
			this.dontShowChild = true;
			bool success = false;
			switch( hint )
			{
				case MoveHint.moveNext:
				{
					// This portion of the case shares some common code with the base class.
					if( this.HighlightRange == GridRangeInfo.Empty )
						success = this.MoveSelection( MoveHint.moveFirst );
					else
					{
						int newRow = this.HighlightRange.Top + 1;

						// Skip rows that cannot be selected
						while( newRow <= Model.RowCount &&
						( !IsSelectable( newRow ) || this.parentItem.Items[newRow - 1] is StaticBarItem ) )
							newRow++;

						if( newRow <= Model.RowCount )
						{
							this.HighlightRange = GridRangeInfo.Row( newRow );
							success = true;
						}
						else
						{
							if( partialMenusOn )	// If in partial menus mode, expand.
								this.ExpandPartialMenus( false );
							success = this.MoveSelection( MoveHint.moveFirst );
						}
					}
					break;
				}
				default:
				{
					success = base.MoveSelection( hint );
					break;
				}
			}
			this.dontShowChild = false;
			return success;
		}
		public virtual Control GetPopupParentControl()
		{
			if( this.parentMenuUI != null )
				return this.parentMenuUI.GetPopupParentControl();
			else
				return null;
		}

		internal bool ShowShadow
		{
			get
			{
				BarRenderer renderer = this.PopupParent as BarRenderer;
				if( renderer != null && renderer.Bar != null && renderer.Bar.Manager != null )
				{
					return renderer.Bar.Manager.ShowShadow;
				}

				MenuGrid grid = this.PopupParent as MenuGrid;
				if( grid != null )
				{
					return grid.ShowShadow;
				}

				PopupMenu menu = this.PopupParent as PopupMenu;
				if( menu != null && menu != null )
				{
					CommandBarExt commandBar = menu.PopupParent as CommandBarExt;
					if( commandBar != null && commandBar.Bar != null && commandBar.Bar.Manager != null )
					{
						return commandBar.Bar.Manager.ShowShadow;
					}
				}

				ToolbarListPopupMenu toolbarList = this.PopupParent as ToolbarListPopupMenu;

				if( toolbarList != null && toolbarList.ParentBarItem.barManager != null )
				{
					return toolbarList.ShowShadow;
				}

				return false;
			}
		}

		public IPopupParent PopupParent
		{
			get { return this.parentMenuUI; }
		}
		public Point[] GetBorderOverlapCue( PopupRelativeAlignment rAlign )
		{
			return null;
		}
		public void OnChildClosed( PopupCloseType popupCloseType, bool fromChildControl )
		{
			this.childMenuUI = null;
			this.curDropDownItem = null;
			this.currentVisibleChildIndex = -1;
			//			if((this.parentItem.Manager != null && this.parentItem.Manager.useHooksForMenus)
			//				|| (this.parentItem.Manager == null && this.parentItem.DesignMode)
			//				// AllowQuit == false means this is hosted by a native windows app.
			//				|| (!this.parentItem.DesignMode && Application.AllowQuit == false)
			//				)
			//			{
			//				this.MouseHooker.HookMessages = true;
			//				this.KeyboardHooker.HookMessages = true;
			//			}
			if( this.parentItem != null )
				PopupManager.SetCurrentPopupClient( this, true,
					false
					);

			if( this.childMenuUI == null )
				this.StartListeningForDeactivation();

			if( popupCloseType != PopupCloseType.Canceled )
				this.HidePopup( popupCloseType );
		}

		public virtual void ChildClosing( IPopupChild childUI, PopupCloseType popupCloseType )
		{
			if( this.MenuBound && this.childMenuUI == childUI )
				this.OnChildClosed( popupCloseType, false );
		}

		public Form GetOwnerForm()
		{
			return this.gridHost;
		}
		public bool IsShowing()
		{
			if( this.aboutToShow )
				return aboutToShow;

			if( this.gridHost != null )
				return this.gridHost.IsShowing();
			else
				return false;
		}

		private MainFrameBarManager GetMainManager()
		{
			if( parentMenuUI == null ) return null;

			PopupMenu menu = this.parentMenuUI as PopupMenu;
			if( menu == null ) return null;
			if( menu.ParentBarItem == null ) return null;

			MainFrameBarManager barManager = menu.ParentBarItem.Manager as MainFrameBarManager;
			if( barManager != null ) return barManager;

			ChildFrameBarManager childMan = menu.ParentBarItem.Manager as ChildFrameBarManager;
			if( childMan != null )
			{
				barManager = childMan.MainFrameBarManager;
			}

			return barManager;
		}

		public override void HidePopup( PopupCloseType popupCloseType )
		{
			lastkeychar = ' ';
			if( !this.MenuBound )
				return;

			CancelEventArgs e = new CancelEventArgs();

			if( this.parentItem != null )
			{
				this.parentItem.OnPopupClosing( e );

				if( e.Cancel )
				{
					return;
				}
			}

			this.IsClicking = false;

			if (this.CurrentCell.Renderer != null && this.CurrentCell.Renderer.Control is GridDropDownEditPartControl)
			{
				this.CurrentCell.Deactivate(false);
			}
			else
			{
				this.CurrentCell.Deactivate(true);
			}

			ParentBarItem oldParentItem = this.parentItem;

			BarItem itemClicked = this.clickedItem;

			IPopupParent parentUI = this.parentMenuUI;

			IPopupChild childUI = this.childMenuUI;

			MainFrameBarManager mainBarMan = GetMainManager();
			if( mainBarMan != null )
			{
				mainBarMan.ShouldHidePopup = false;
			}

			this.MenuBound = false;

			if( this.gridHost != null && this.gridHost.IsShowing() )
			{
				//this.gridHost.Visible = false;
				this.gridHost.HidePopup();
				this.Visible = false;
			}

			if( childUI != null )
			{
				PopupControlContainer popup = childUI as PopupControlContainer;
				if( popup == null || !( popup.IgnoreMouseMessages && m_bIsMouseMessage ) )
				{
					childUI.HidePopup( popupCloseType );
				}
			}

			if( parentUI != null )
				parentUI.ChildClosing( this, popupCloseType );

			if( oldParentItem != null )
				oldParentItem.OnPopupClosed( EventArgs.Empty );

			if( itemClicked != null )
				this.NotifyItem( itemClicked );

			if( !this.dndHelper.Dragging )
				this.ReleaseGrid();

            this.parentMenuUI = null;
		}
		protected virtual void ReleaseGrid()
		{
			this.FreeObjectCache();
			XPMenuGridFactory.ReleaseMenuGrid( this );
		}
		#endregion GRID_OVERRIDES



		#region Overrides

		protected override void OnChildCollectionChanged( object sender, CollectionChangeEventArgs e )
		{
			base.OnChildCollectionChanged( sender, e );

			BarItems items = sender as BarItems;

			if( null != items && items.Count > 0 )
			{
				RefreshMenuGrid( false );
			}
		}

		#endregion Overrides
	}
    public interface IMenuGrid
    {
        MenuGrid menuGrid
        {
            get;
            set;
        }
    }
}
