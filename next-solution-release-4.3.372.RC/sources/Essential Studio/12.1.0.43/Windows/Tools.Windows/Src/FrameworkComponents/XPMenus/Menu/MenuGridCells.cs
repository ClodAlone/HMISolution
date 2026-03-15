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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.Styles;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Win32;
using Syncfusion.ComponentModel;
using Syncfusion.Runtime.InteropServices;
#endregion

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	internal class TextBoxCellModel: GridTextBoxCellModel
	{
		public TextBoxCellModel( GridModel grid )
			: base( grid )
		{
			this.AllowFloating = true;
		}

		public override GridCellRendererBase CreateRenderer( GridControlBase control )
		{
			return new TextBoxCellRenderer( control, this );
		}

		public override bool OnQueryCanFloatCell( int rowIndex, int colIndex, GridStyleInfo style, GridQueryFloatCell query )
		{
			return true;
		}
	}

	internal class TextBoxCellRenderer: GridTextBoxCellRenderer, IFocusableRenderer
	{
		public TextBoxCellRenderer( GridControlBase grid, GridCellModelBase model )
			: base( grid, model )
		{
		}

		bool IFocusableRenderer.IsRelatedControl( Control control )
		{
			if( control == this.TextBox )
				return true;
			else
				return false;
		}

		public override void Draw( Graphics g, Rectangle cellRectangle, int rowIndex, int colIndex, GridStyleInfo style )
		{
			bool selected = false;

			MenuGridControlBase grid = Grid as MenuGridControlBase;
			if( grid != null )
			{
				selected = grid.HighlightRange.Top == rowIndex;
				m_bDrawSelectedBackground = selected;
			}

			m_tbItem = (TextBoxBarItem)grid.parentItem.Items[rowIndex - 1];

			if( !m_tbItem.ReiseBeforePopupItemPaint( g, cellRectangle, ref selected, DrawElement.TextBox, style ) )
			{
				base.Draw( g, cellRectangle, rowIndex, colIndex, style );
			}

			m_tbItem.ReiseAfterPopupItemPaint( g, cellRectangle, selected, DrawElement.TextBox, style );
		}

		/// <summary>
		/// Indicates whether must be drawing highlight background.
		/// </summary>
		private bool m_bDrawSelectedBackground = false;

		protected override void DrawBackground( Graphics g, Rectangle rect, GridStyleInfo style, bool fillBackground )
		{
			base.DrawBackground( g, rect, style, fillBackground );

			MenuGridControlBase grid = Grid as MenuGridControlBase;

			if( grid != null && ( grid.Style == VisualStyle.Office2007 && !( grid.ThemesEnabled && grid.IsVistaOS ) ) )
			{
				Office2007MenuPainter.DrawMenuTextBoxItem( g, rect, m_bDrawSelectedBackground );
			}
            else if (grid != null && (grid.Style == VisualStyle.Office2010 && !(grid.ThemesEnabled && grid.IsVistaOS)))
            {
                Office2010MenuPainter.DrawMenuTextBoxItem(g, rect, m_bDrawSelectedBackground);
            }
		}

		private TextBoxBarItem m_tbItem;

		protected override TextBoxBase CreateTextBox()
		{
			TextBoxBase textBoxBase = base.CreateTextBox();

			textBoxBase.HandleCreated += new EventHandler( TextBoxHandleCreated );

			return textBoxBase;
		}

		void TextBoxHandleCreated( object sender, EventArgs e )
		{
			TextBoxBase textBoxBase = (TextBoxBase)sender;

			m_tbItem.OnTextBoxItemBound( new TextBoxItemBoundEventArgs( textBoxBase ) );
		}
	}

	internal class MenuComboBoxCellModel: GridComboBoxCellModel
	{
		#region PRIVATE_DATA
		private bool supportsTextBox;
		private MenuComboBoxCellRenderer myRenderer = null;
		#endregion PRIVATE_DATA
		public MenuComboBoxCellModel( GridModel grid, bool supportsTextBox )
			: base( grid )
		{
			this.supportsTextBox = supportsTextBox;
			this.AllowFloating = true;
			this.SupportsChoiceList = true;
			this.ButtonBarSize = new Size( this.ButtonBarSize.Width - 4, this.ButtonBarSize.Height );
		}

		public override GridCellRendererBase CreateRenderer( GridControlBase control )
		{
			this.myRenderer = new MenuComboBoxCellRenderer( control, this, this.supportsTextBox );
			return this.myRenderer;
		}

		public override bool OnQueryCanFloatCell( int rowIndex, int colIndex, GridStyleInfo style, GridQueryFloatCell query )
		{
			return true;
		}
		public override void FillWithChoices( ListBox listBox, GridStyleInfo style, out bool exclusive )
		{
			MenuGrid grid = this.myRenderer.Grid as MenuGrid;
			if( grid != null && !grid.Customizing )
			{
				if( this.myRenderer.RowIndex > 0
					&& grid.parentItem.Items.Count > this.myRenderer.RowIndex - 1 )
				{
					ComboBoxBarItem barItem = grid.parentItem.Items[this.myRenderer.RowIndex - 1] as ComboBoxBarItem;
					if( barItem.ListBox != null )
					{
						exclusive = style.ExclusiveChoiceList;
						return;
					}
				}
			}
			base.FillWithChoices( listBox, style, out exclusive );
		}
	}

	internal class MenuComboBoxCellRenderer: GridComboBoxCellRenderer, IPopupChild, IFocusableRenderer,
		IMouseHookHLProcClient, IKeyboardProcHookClient
	{
		#region PRIVATE_DATA
		internal AutoAppend autoAppend;
		private ListBox cached = null;
		private ComboBoxBarItem m_comboBarItem = null;
		#endregion PRIVATE_DATA

		public MenuComboBoxCellRenderer( GridControlBase grid, GridCellModelBase model
			, bool supportsTextBox )
			: base( grid, model )
		{
			this.DisableTextBox = !supportsTextBox;
			this.SupportsFocusControl = supportsTextBox;
			DropDownButton = new MenuComboButton( this );
		}

		public override void DropDownContainerCloseDropDown( object sender, PopupClosedEventArgs e )
		{
			base.DropDownContainerCloseDropDown( sender, e );

			MenuComboButton comboButton = DropDownButton as MenuComboButton;

			if( comboButton != null )
			{
				comboButton.m_bDroppedDown = false;
			}

			ComboBoxBarItem item = this.BarItem;
			
			if (item != null)
			{
				item.OnDropDownClosed();
			}
		}

		/// <override/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				m_comboBarItem = null;
			}

			base.Dispose( disposing );
		}

		ComboBoxBarItem BarItem
		{
			get
			{
				return m_comboBarItem;
			}
		}

		bool IFocusableRenderer.IsRelatedControl( Control control )
		{
			if( control == this.EditPart || control == this.PopupControlContainer
				|| ( this.PopupControlContainer != null && this.PopupControlContainer.Contains( control ) )
				)
				return true;
			else
				return false;
		}

		#region IPopupChild_Imp
		void IPopupChild.HidePopup( PopupCloseType popupCloseType )
		{
		}

		bool IPopupChild.IsShowing()
		{
			return true;
		}

		IPopupParent IPopupChild.PopupParent
		{
			get { return Grid as IPopupParent; }
		}
		bool INeedKeyboardMessages.KeyboardMessage( ref Message m )
		{
			return false;
		}

		bool INeedMouseMoveMessages.MouseMessage( ref Message m )
		{
			return false;
		}

		bool IMouseHookHLProcClient.MouseHookProc( int msg, Point point, IntPtr hwnd, int wHitTestCode, int dwExtraInfo )
		{
			return false;
		}

		bool IKeyboardProcHookClient.KeyboardHookProc( int wParam, int lParam )
		{
			return false;
		}
		#endregion IPopupChild_Imp
		//
		//		protected override void OnEditPartKeyPress(object sender, KeyPressEventArgs e)
		//		{
		//			// Don't want base class behavior. Base class checks for partial matches and fills the text box with the matched text.
		//		}

		protected override void OnKeyDown( KeyEventArgs e )
		{
			bool preventDefaultBehavior = false;
			switch( e.KeyCode )
			{
				case Keys.Home:
				case Keys.End:
				preventDefaultBehavior = true;
				break;
				case Keys.Enter:
				Grid.CurrentCell.ConfirmChanges();
				break;
				case Keys.Escape:
				Grid.CurrentCell.Deactivate( true );
				break;
			}
			if( !preventDefaultBehavior )
			{
				bool wasDroppedDown = this.IsDroppedDown;
				base.OnKeyDown( e );
				if( wasDroppedDown && e.KeyCode == Keys.Return )
				{
					// Trying to let the framework know that this message was processed
					// so it will not generate a corresponding WM_CHAR. But, this setting is
					// ignored and a corresponding WM_CHAR does get generated, which gets
					// processed by the MenuGrid resulting in closing the menu.
					e.Handled = true;
				}
			}
		}

		public override void ChildClosing( IPopupChild childUI, PopupCloseType popupCloseType )
		{
			base.ChildClosing( childUI, popupCloseType );

			// Using the OnChildClosed method, that usually indicates closing of a submenu...
			// We still haven't fired the event, so will not Close the menu already.
			PopupCloseType originalCloseType = popupCloseType;
			if( popupCloseType == PopupCloseType.Done )
				popupCloseType = PopupCloseType.Canceled;

			( (MenuGrid)Grid ).OnChildClosed( popupCloseType, true );

			// Save changes in combo box before firing the event.
			Grid.CurrentCell.ConfirmChanges();
			if( originalCloseType == PopupCloseType.Done )
			{
				( (MenuGrid)Grid ).ProcessItemClick( BarItem );
			}
		}

		public override bool IsRelatedControl( Control control, bool askPopupParent )
		{
			if( control == this.Grid || control == this.EditPart
				|| control.Parent == this.Grid )
				return true;
			else if( askPopupParent )
				return ( (MenuGrid)this.Grid ).IsRelatedControl( control, askPopupParent );
			else
				return false;
		}

		protected override void ListBoxClick( object sender, EventArgs e )
		{
			// Idea is not to call the base class.
		}

		public override void Draw( Graphics g, Rectangle cellRectangle, int rowIndex, int colIndex, GridStyleInfo style )
		{
			bool selected = false;
			MenuGridControlBase grid = Grid as MenuGridControlBase;
			if( grid != null )
			{
				selected = grid.HighlightRange.Top == rowIndex;
				m_bDrawSelectedBackground = selected;
			}

			ComboBoxBarItem comboBarItem = (ComboBoxBarItem)grid.parentItem.Items[rowIndex - 1];

			if (!comboBarItem.ReiseBeforePopupItemPaint(g, cellRectangle, ref selected, DrawElement.ComboBox, style))
			{
				base.Draw( g, cellRectangle, rowIndex, colIndex, style );
			}

			comboBarItem.ReiseAfterPopupItemPaint(g, cellRectangle, selected, DrawElement.ComboBox, style, this);
		}

		/// <summary>
		/// Indicates whether must be drawing highlight background.
		/// </summary>
		private bool m_bDrawSelectedBackground = false;

		protected override void DrawBackground( Graphics g, Rectangle rect, GridStyleInfo style, bool fillBackground )
		{
			base.DrawBackground( g, rect, style, fillBackground );

			MenuGridControlBase grid = Grid as MenuGridControlBase;

			if( grid != null && ( grid.Style == VisualStyle.Office2007 && !( grid.ThemesEnabled && grid.IsVistaOS ) ) )
			{
				Office2007MenuPainter.DrawMenuTextBoxItem( g, rect, m_bDrawSelectedBackground );
			}
            else if (grid != null && (grid.Style == VisualStyle.Office2010 && !(grid.ThemesEnabled && grid.IsVistaOS)))
            {
                Office2010MenuPainter.DrawMenuTextBoxItem(g, rect, m_bDrawSelectedBackground);
            }
            if( grid != null && ( grid.Style == VisualStyle.Metro && !( grid.ThemesEnabled && grid.IsVistaOS )))
            {
                MetroMenuPainter.DrawMenuTextBoxItem(g, rect, m_bDrawSelectedBackground);
            }
		}

		protected override void OnInitialize( int rowIndex, int colIndex )
		{
			// Call ApplyAutoAppendSettings before calling base so that the Items list
			// gets initialized in advance.
			MenuGrid grid = Grid as MenuGrid;
			if( grid != null && !grid.Customizing )
			{
				if( rowIndex > 0
					&& grid.parentItem.Items.Count > rowIndex - 1 )
				{
					if( null != this.BarItem )
					{
						if( null != this.BarItem.ListBox )
						{
							this.cached = this.ListBoxPart;
							this.ListBoxPart = BarItem.ListBox;
						}

						PropagateRTL( grid.IsRTL ? RightToLeft.Yes : RightToLeft.No );
					}
				}

				m_comboBarItem = grid.parentItem.Items[rowIndex - 1] as ComboBoxBarItem;

				this.ApplyAutoAppendSettings();
			}
			base.OnInitialize( rowIndex, colIndex );
		}

		protected void PropagateRTL( RightToLeft eRTL )
		{
			if( null != this.ListBoxPart ) this.ListBoxPart.RightToLeft = eRTL;
			if( null != this.EditPart ) this.EditPart.RightToLeft = eRTL;
		}

		protected void PropagateRTL()
		{
			MenuGrid grid = (MenuGrid)this.Grid;

			if( null != grid )
			{
				RightToLeft eRTL = grid.IsRTL ? RightToLeft.Yes : RightToLeft.No;
				PropagateRTL( eRTL );
			}
		}

		protected override void OnDeactived( int rowIndex, int colIndex )
		{
			if( this.cached != null )
			{
				this.ListBoxPart = this.cached;
				this.cached = null;
			}
			base.OnDeactived( rowIndex, colIndex );
		}
		protected override bool OnSaveChanges()
		{
			bool retVal = base.OnSaveChanges();
			// If changes were saved...
			if( retVal &&BarItem!=null)
			{
				// If changes were save, ensure that the new value is included in the
				// autoappend list. (The Validated message is not being caught by the AutoAppend controller.)
				if( BarItem.AutoAppend )
				{
					this.autoAppend.InsertOrMoveToTop( this.TextBox, this.TextBox.Text );
				}
			}
			if( this.autoAppend != null )
			{
				this.autoAppend.Dispose();
				this.autoAppend = null;
			}
			return retVal;
		}

		protected virtual void ApplyAutoAppendSettings()
		{
			MenuGrid grid = Grid as MenuGrid;
			if( grid != null && !grid.Customizing )
			{
				if( BarItem.AutoAppend )
				{
					if( this.autoAppend == null )
						this.autoAppend = new AutoAppend();
					BarItemID barItemID = new BarItemID( BarItem.ID,
						BarItem.Manager != null ?
						BarManager.GetFormTypeName( BarItem.Manager ) : String.Empty );
					this.autoAppend.SetAutoAppend( this.TextBox, new AutoAppendInfo( true, barItemID.ToString(), BarItem.ChoiceList, 30 ) );
				}
				else
				{
					if( this.autoAppend != null )
						this.autoAppend.SetAutoAppend( this.TextBox, new AutoAppendInfo( false, String.Empty, null, 30 ) );
				}
			}
		}

		protected override void TextBoxLostFocus( object sender, EventArgs e )
		{
			// If the dropdown container is the one that got the focus, then don't relay this message.
			if( this.DropDownContainer.ContainsFocus )
				return;

			base.TextBoxLostFocus( sender, e );
		}
		public override void DropDownContainerShowingDropDown( object sender, CancelEventArgs e )
		{
			MenuGrid grid = Grid as MenuGrid;

			if( grid != null && !grid.Customizing )
			{
				// Provide the user a chance to add/remove items into the listbox.
				BarItem.OnInitListBox( new ComboBoxBarItemInitListBoxEventArgs( this.ListBoxPart ) );

				base.DropDownContainerShowingDropDown( sender, e );

				GridComboBoxListBoxPart dropDown = ListBoxPart as GridComboBoxListBoxPart;
				IIgnoreWorkingArea iwa = BarItem as IIgnoreWorkingArea;

				if( iwa != null && DropDownContainer != null &&
					this.DropDownContainer.PopupHost != null )
				{
					this.DropDownContainer.PopupHost.IgnoreWorkingArea = true;
				}
				else
				{
					this.DropDownContainer.PopupHost.IgnoreWorkingArea = false;
				}

				if( dropDown != null )
				{
					dropDown.DropDownRows = BarItem.MaxDropDownItems;
				}

				ListBoxPart.SelectedItem = BarItem.TextBoxValue;

				int width = this.GetCellBoundsCore( this.CurrentCell.RowIndex, this.CurrentCell.ColIndex ).Width;
				if( width < BarItem.MinDropDownWidth )
					width = BarItem.MinDropDownWidth;

				ListBoxPart.Width = width;

				if( BarItem.MaxDropDownItems <= this.ListBoxPart.Items.Count )
				{
					int listHeight = 0;
					if( this.ListBoxPart.DrawMode != DrawMode.OwnerDrawVariable )
					{
						if( BarItem.MaxDropDownItems > 0 )
							listHeight = this.ListBoxPart.GetItemHeight( 0 );
						listHeight = BarItem.MaxDropDownItems * listHeight;
					}
					else
					{
						for( int i = 0; i < BarItem.MaxDropDownItems; i++ )
						{
							listHeight += this.ListBoxPart.GetItemHeight( i );
						}
					}
					this.ListBoxPart.Height = listHeight;
				}
				else
					this.ListBoxPart.Height = this.ListBoxPart.PreferredHeight;

				Size size = ListBoxPart.Size;
				this.DropDownContainer.Size = size;

				ListBoxPart.Visible = true;

				ComboBoxBarItem item = this.BarItem;
				
				if (item != null)
				{
					item.OnDropDownOpened();
				}
			}
		}

		protected override void OnButtonClicked( int rowIndex, int colIndex, int button )
		{
			MenuGrid grid = Grid as MenuGrid;

			// Prevent calling base class when not a MenuGrid and customizing
			if( grid != null && !grid.Customizing )
			{
				base.OnButtonClicked( rowIndex, colIndex, button );
			}
			return;
		}
		protected override void OnClick( int rowIndex, int colIndex, MouseEventArgs e )
		{
			MenuGrid grid = Grid as MenuGrid;

			// Prevent calling base class when not a MenuGrid and customizing
			if( grid != null && !grid.Customizing )
			{
				base.OnClick( rowIndex, colIndex, e );
			}
			return;
		}

		protected override TextBoxBase CreateTextBox()
		{
			TextBoxBase textBoxBase = base.CreateTextBox();

			textBoxBase.HandleCreated += new EventHandler( TextBoxHandleCreated );

			return textBoxBase;
		}

		void TextBoxHandleCreated( object sender, EventArgs e )
		{
			TextBoxBase textBoxBase = (TextBoxBase)sender;
            if (BarItem != null)
            {
                BarItem.OnTextBoxBound(new TextBoxBoundEventArgs(textBoxBase));
            }
		}
	}

	internal class MenuComboButton: GridCellComboBoxButton
	{
		public MenuComboButton( GridCellRendererBase control )
			: base( control )
		{
		}
		// Logic to drop-down the list box even when clicked outside the button (in listbox mode).
		public override int HitTest( int rowIndex, int colIndex,
			MouseEventArgs e, Syncfusion.Windows.Forms.IMouseController controller )
		{
			MenuGrid grid = Grid as MenuGrid;

			if( grid != null && !grid.Customizing )
			{
				ComboBoxBarItem barItem = null;

				if( rowIndex > 0 && rowIndex <= grid.parentItem.Items.Count )
					barItem = grid.parentItem.Items[rowIndex - 1] as ComboBoxBarItem;

				if( barItem != null && !barItem.Editable )
					return GridHitTestContext.CellButtonElement;
				else
					return base.HitTest( rowIndex, colIndex, e, controller );
			}
			else
				return GridHitTestContext.None;
		}

		public override void Draw( Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style )
		{
			// draw the button
			MenuGridControlBase menuGrid = Grid as MenuGridControlBase;
			VisualStyle visualStyle = ( menuGrid == null ) ? VisualStyle.Default : menuGrid.Style;

            if ((visualStyle == VisualStyle.Office2007 || visualStyle == VisualStyle.Office2010) && !(menuGrid.ThemesEnabled && menuGrid.IsVistaOS))
			{
				if( menuGrid != null )
				{
					m_bDrawSelectedBackground = ( menuGrid.HighlightRange.Top == rowIndex );
				}

				// draw ComboButton with Office2007 visual style
				base.Draw( g, rowIndex, colIndex, bActive, style );
			}
            else if (visualStyle == VisualStyle.Metro)
            {
                if (menuGrid != null)
                {
                    m_bDrawSelectedBackground = (menuGrid.HighlightRange.Top == rowIndex);
                }
                base.Draw(g, rowIndex, colIndex, bActive, style);
            }
            else
            {
                bool bDisabled = !style.Clickable;
                bool bRTL = this.Grid.IsRightToLeft();
                bool bMouseDown = IsMouseDown(rowIndex, colIndex);
                bool bHovering = !menuGrid.Customizing && (menuGrid.HighlightRange.Top == rowIndex);

				Rectangle adjustedRect = GetAdjustedRect( bHovering, bRTL );
				Rectangle brushRect = GetBrushRect();
				Brush brush = null;

				if( bDisabled )
				{
					Color backColor = GetComboButtonHighlightColor( visualStyle );
					backColor = Color.FromArgb( 125, backColor );
					brush = new SolidBrush( backColor );
				}
				else
				{
					if( menuGrid.ThemesEnabled && menuGrid.IsVistaOS )
					{
						brush = ( bHovering ) ? new LinearGradientBrush( adjustedRect, VistaMenuColors.SelBGColorLight, VistaMenuColors.SelBGColorDark, LinearGradientMode.Vertical ) as Brush :
							new SolidBrush( menuGrid.BackColor ) as Brush;
					}
					else
					{
						brush = ( bHovering ) ? GetComboButtonHighlightBrush( visualStyle, adjustedRect ) :
							GetComboButtonBrush( visualStyle, brushRect );
					}
				}

				adjustedRect.Y -= 1;
				adjustedRect.Height += 3;

				using( brush )
				{
					g.FillRectangle( brush, adjustedRect );
				}

				if( bHovering )
				{
					Pen pen;
					if( menuGrid.ThemesEnabled && menuGrid.IsVistaOS )
					{
						pen = new Pen( VistaMenuColors.SelBorderColor );
					}
					else
					{
						pen = GetComboButtonBorderPen( visualStyle );
					}

					using( pen )
					{
						g.DrawRectangle( pen, adjustedRect );
					}
				}

				// The drop-down arrow.
				Rectangle arrowRect = GetArrowRect( bRTL );
				DrawDropDownArrow( g, arrowRect, bDisabled );
			}
		}

		/// <summary>
		/// Default offset for right-ot-left drawing.
		/// </summary>
		private const int c_offsetRTL = 1;

		/// <summary>
		/// Gets adjusted rectangle for button.
		/// </summary>
		private Rectangle GetAdjustedRect( bool bHovering, bool bRTL )
		{
			Rectangle rect = this.Bounds;

			if( bHovering )
			{
				if( bRTL )
				{
					rect.X -= 2;
					rect.Width += c_offsetRTL;
				}
                rect.Y -= 1;
                //rect.Y -= 2;
				rect.Height += 3;
				rect.Width += c_offsetRTL;
			}
			else if( !bRTL )
			{
				rect.X += c_offsetRTL;
				rect.Width -= c_offsetRTL;
			}

			return rect;
		}


		/// <summary>
		/// Gets rectangle of the gradient brush for fill ComboButton.
		/// </summary>
		private Rectangle GetBrushRect()
		{
			Rectangle rect = this.Bounds;
			rect.Y -= c_offsetRTL;

			return rect;
		}


		/// <summary>
		/// Gets rectangle for arrow of the ComboButton.
		/// </summary>
		private Rectangle GetArrowRect( bool bRTL )
		{
			Rectangle rect = this.Bounds;

			if( bRTL )
			{
				rect.X -= c_offsetRTL;
			}

			return rect;
		}

		/// <summary>
		/// Draws DropDown arrow.
		/// </summary>
		private void DrawDropDownArrow( Graphics g, Rectangle rect, bool disabled )
		{
			Point[] ptsscrll = ComboBoxItemRenderer.GetDropDownBorderBounds( rect );

			using( GraphicsPath gpath = new GraphicsPath() )
			{
				gpath.AddLines( ptsscrll );

				using( Brush brush = new SolidBrush( disabled ? Color.FromArgb( 125, SystemColors.ControlText ) : SystemColors.ControlText ) )
				using( Region reg = new Region( gpath ) )
				{
					g.FillRegion( brush, reg );
				}
			}
		}

		public override void DrawButton( Graphics g, Rectangle rect, ButtonState buttonState, GridStyleInfo style )
		{
			MenuGridControlBase menuGrid = Grid as MenuGridControlBase;

			if( menuGrid != null && menuGrid.Style == VisualStyle.Office2007 )
			{
				rect.Y -= 1;
				rect.Height += 3;

				bool bRTL = this.Grid.IsRightToLeft();

				if( bRTL )
				{
					rect.X -= 1;
				}

				Office2007MenuPainter.DrawComboButton( g, rect, buttonState, m_bDroppedDown, m_bDrawSelectedBackground, bRTL );
			}
            else if( menuGrid != null && menuGrid.Style == VisualStyle.Office2010 )
			{
				rect.Y -= 1;
				rect.Height += 3;

				bool bRTL = this.Grid.IsRightToLeft();

				if( bRTL )
				{
					rect.X -= 1;
				}

				Office2010MenuPainter.DrawComboButton( g, rect, buttonState, m_bDroppedDown, m_bDrawSelectedBackground, bRTL );
			}
            else if (menuGrid != null && menuGrid.Style == VisualStyle.Metro)
            {
                rect.Y -= 1;
                rect.Height += 3;
                bool bRTL = this.Grid.IsRightToLeft();
                if (bRTL)
                {
                    rect.X -= 1;
                }
                MetroMenuPainter.DrawComboButton(g, rect, buttonState, m_bDroppedDown, m_bDrawSelectedBackground, bRTL);
            }
		}

		/// <summary>
		/// Indicates whether must be drawing highlight background.
		/// </summary>
		private bool m_bDrawSelectedBackground = false;

		/// <summary>
		/// Indicates whether the drop-down is currently dropped-down and visible.
		/// </summary>
		internal bool m_bDroppedDown = false;

		protected override void OnClicked( GridCellEventArgs e )
		{
			m_bDroppedDown = !m_bDroppedDown;
			base.OnClicked( e );
		}


		/// <summary>
		/// Gets color of the combo button amenably with VisualStyle.
		/// </summary>
		private Color GetComboButtonHighlightColor( VisualStyle style )
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
					color = Office2007OutlookColors.MenuSelectedItemColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = VS2005Colors.MenuSelectedItemColor;
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
		/// Gets brush of the highligh combo button amenably with VisualStyle.
		/// </summary>
		private Brush GetComboButtonHighlightBrush( VisualStyle style, Rectangle rect )
		{
			Brush brush = null;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					brush = new SolidBrush( Office2003Colors.SelColor );
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					brush = new LinearGradientBrush( rect, Office2007OutlookColors.DropDownHighlightLightColor,
						Office2007OutlookColors.DropDownHighlightDarkColor, LinearGradientMode.Vertical );
					break;
				}
				case VisualStyle.VS2005:
				{
					brush = new LinearGradientBrush( rect, VS2005Colors.DropDownHighlightLightColor,
						VS2005Colors.DropDownHighlightDarkColor, LinearGradientMode.Vertical );
					break;
				}
				default:
				{
					brush = new SolidBrush( MenuColors.SelColor );
					break;
				}
			}

			return brush;
		}

		/// <summary>
		/// Gets brush of the combo button amenably with VisualStyle.
		/// </summary>
		private Brush GetComboButtonBrush( VisualStyle style, Rectangle rect )
		{
			Brush brush = null;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					brush = new SolidBrush( SystemColors.Control );
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					brush = new LinearGradientBrush( rect, Office2007OutlookColors.ComboButtonLightColor,
						Office2007OutlookColors.ComboButtonDarkColor, LinearGradientMode.Vertical );
					break;
				}
				case VisualStyle.VS2005:
				{
					brush = new LinearGradientBrush( rect, VS2005Colors.DDBarItemLightColor,
						VS2005Colors.DDBarItemDarkColor, LinearGradientMode.Vertical );
					break;
				}
				default:
				{
					brush = new SolidBrush( SystemColors.Control );
					break;
				}
			}

			return brush;
		}


		/// <summary>
		/// Gets pen for border of the combo button amenably with VisualStyle.
		/// </summary>
		private Pen GetComboButtonBorderPen( VisualStyle style )
		{
			Pen pen = null;
			MenuGridControlBase grid = Grid as MenuGridControlBase;

			if( grid != null && ( grid.ThemesEnabled && grid.IsVistaOS ) )
			{
				pen = new Pen( VistaMenuColors.SelBorderColor );
			}
			else
			{
				switch( style )
				{
					case VisualStyle.Office2003:
					{
						pen = new Pen( Office2003Colors.SelBorderColor );
						break;
					}

					case VisualStyle.Office2007Outlook:
					{
						pen = new Pen( Office2007OutlookColors.MenuSelectedItemBorderColor );
						break;
					}

					case VisualStyle.VS2005:
					{
						pen = new Pen( VS2005Colors.MenuSelectedItemBorderColor );
						break;
					}

					default:
					{
						pen = new Pen( MenuColors.SelBorderColor );
						break;
					}
				}
			}

			return pen;
		}

	}


	internal class MenuTextCellModel: GridStaticCellModel
	{
		public MenuTextCellModel( GridModel grid )
			: base( grid )
		{
			this.AllowFloating = true;
		}

		public override GridCellRendererBase CreateRenderer( GridControlBase control )
		{
			return new MenuTextCellRenderer( control, this );
		}
		public override bool OnQueryCanFloatCell( int rowIndex, int colIndex, GridStyleInfo style, GridQueryFloatCell query )
		{
			return true;
		}
	}

	internal class MenuTextCellRenderer: GridStaticCellRenderer
	{
		public MenuTextCellRenderer( GridControlBase grid, GridCellModelBase model )
			: base( grid, model )
		{
		}

		public override void Draw( Graphics g, Rectangle cellRectangle, int rowIndex, int colIndex, GridStyleInfo style )
		{
			bool selected = false;

			MenuGridControlBase grid = Grid as MenuGridControlBase;
			if( grid != null )
			{
				selected = grid.HighlightRange.Top == rowIndex;
			}

			BarItem barItem = grid.parentItem.Items[rowIndex - 1];

			if( !barItem.ReiseBeforePopupItemPaint( g, cellRectangle, ref selected, DrawElement.Text, style ) )
			{
				base.Draw( g, cellRectangle, rowIndex, colIndex, style );
			}
			barItem.ReiseAfterPopupItemPaint( g, cellRectangle, selected, DrawElement.Text, style );
		}

		protected override void OnDraw( Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style )
		{
			string displayText = String.Empty;

			if( clientRectangle.IsEmpty )
				return;

			Rectangle textRectangle = RemoveMargins( clientRectangle, style );
			if( textRectangle.IsEmpty )
				return;

			try
			{
				if( this.ShouldDrawEditing( rowIndex, colIndex ) )
					displayText = this.ControlText;
				else
					displayText = Model.GetFormattedOrActiveTextAt( rowIndex, colIndex, style );
			}
			catch( Exception ex )
			{
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					throw ex;

				displayText = style.Text;
			}

			if( style.HasError )
			{
				displayText = style.Error;
			}

			if( displayText.Length > 0 )
			{
				GridDrawCellDisplayTextEventArgs e = new GridDrawCellDisplayTextEventArgs( g, displayText, textRectangle, style );
				Grid.RaiseDrawCellDisplayText( e );
				if( !e.Cancel )
				{
					textRectangle = e.TextRectangle;
					displayText = e.DisplayText;
					Font font = style.GdipFont;
					Color textColor = Grid.PrintingMode && Grid.Model.Properties.BlackWhite ? Color.Black : style.TextColor;
					bool isTextRightToLeft = style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft() || style.RightToLeft == RightToLeft.Yes;

					int nFlags = ControlDrawing.DrawTextFlags;
					nFlags &= ~DrawTextFormats.DT_NOPREFIX;

					if( style.HotkeyPrefix == System.Drawing.Text.HotkeyPrefix.Hide )
					{
						nFlags |= DrawTextFormats.DT_HIDEPREFIX;
					}

					IntPtr hdc = g.GetHdc();
					IntPtr hFont = font.ToHfont();
                    try
                    {
                        IntPtr prevFont = NativeMethods.SelectObject(hdc, hFont);

                        Color invertedColor = Color.FromArgb(0, textColor.B, textColor.G, textColor.R);
                        NativeMethods.SetTextColor(hdc, invertedColor.ToArgb() & 0xFFFFFF);
                        NativeMethods.SetBkMode(hdc, 1); // TRANSPARENT

                        NativeMethods.RECT rect = new NativeMethods.RECT(textRectangle);

                        if (isTextRightToLeft)
                        {
                            nFlags |= DrawTextFormats.DT_RTLREADING;
                        }

                        TextFormatFlags flags = TextFormatFlags.Bottom | TextFormatFlags.WordBreak;
                        switch (style.HorizontalAlignment)
                        {
                            case GridHorizontalAlignment.Left:
                                nFlags |= isTextRightToLeft ? DrawTextFormats.DT_RIGHT : DrawTextFormats.DT_LEFT;
                                flags |= TextFormatFlags.Left;
                                break;

                            case GridHorizontalAlignment.Right:
                                nFlags |= isTextRightToLeft ? DrawTextFormats.DT_LEFT : DrawTextFormats.DT_RIGHT;
                                flags |= TextFormatFlags.Right;
                                break;

                            default:
                                nFlags |= DrawTextFormats.DT_CENTER;
                                flags |= TextFormatFlags.HorizontalCenter;
                                break;
                        }

                        int rect_width = TextRenderer.MeasureText(displayText, font).Width;
                        Rectangle rectangle = new Rectangle();
                        rectangle.Width = rect_width;
                        rectangle.X = rect.left;
                        rectangle.Y = rect.top;
                        rectangle.Height = rect.Height;
                        g.ReleaseHdc(hdc);

                        TextRenderer.DrawText(g, displayText, font, rectangle, textColor, flags);

                        //NativeMethods.DrawText(hdc, displayText, displayText.Length, ref rect, nFlags);

                        prevFont = NativeMethods.SelectObject(hdc, prevFont);
                    }
                    finally
                    {
                        NativeMethods.DeleteObject(hFont);

                        //g.ReleaseHdc(hdc);
                    }
				}
			}
		}

		protected override void DrawBackground( Graphics g, Rectangle rect, GridStyleInfo style, bool fillBackground )
		{
			MenuGridControlBase grid = Grid as MenuGridControlBase;

			if( grid != null && ( grid.ThemesEnabled && grid.IsVistaOS ) )
			{
				//Need this to avoid painting background over rounded borders
				//of this cell, which are drawing before filling background.
				rect.Inflate( 0, -2 );
			}

			base.DrawBackground( g, rect, style, fillBackground );
		}
	}

	internal class ShortcutCellModel: GridStaticCellModel
	{
		public ShortcutCellModel( GridModel grid )
			: base( grid )
		{
			this.AllowFloating = true;
		}

		public override GridCellRendererBase CreateRenderer( GridControlBase control )
		{
			return new ShortcutCellRenderer( control, this );
		}
		public override bool OnQueryCanFloatCell( int rowIndex, int colIndex, GridStyleInfo style, GridQueryFloatCell query )
		{
			return true;
		}
	}

	internal class ShortcutCellRenderer: GridStaticCellRenderer
	{
		public ShortcutCellRenderer( GridControlBase grid, GridCellModelBase model )
			: base( grid, model )
		{
		}

		public override void Draw( Graphics g, Rectangle cellRectangle, int rowIndex, int colIndex, GridStyleInfo style )
		{
			bool selected = false;

			MenuGridControlBase grid = Grid as MenuGridControlBase;
			if( grid != null )
			{
				selected = grid.HighlightRange.Top == rowIndex;
			}

			BarItem barItem = grid.parentItem.Items[rowIndex - 1];

			if( !barItem.ReiseBeforePopupItemPaint( g, cellRectangle, ref selected, DrawElement.Shortcut, style ) )
			{
				base.Draw( g, cellRectangle, rowIndex, colIndex, style );
			}
			barItem.ReiseAfterPopupItemPaint( g, cellRectangle, selected, DrawElement.Shortcut, style );
		}

		protected override void DrawBackground( Graphics g, Rectangle rect, GridStyleInfo style, bool fillBackground )
		{
			MenuGridControlBase grid = Grid as MenuGridControlBase;
			if( grid != null && ( grid.ThemesEnabled && grid.IsVistaOS ) )
			{
				//Need this to avoid painting background over rounded borders
				//of this cell, which are drawing before filling background.
				rect.Inflate( 0, -2 );
			}

			base.DrawBackground( g, rect, style, fillBackground );
		}
	}

	internal class MenuGlyphCellModel: GridStaticCellModel
	{
		public MenuGlyphCellModel( GridModel grid )
			: base( grid )
		{
			this.AllowFloating = true;
		}

		public override GridCellRendererBase CreateRenderer( GridControlBase control )
		{
			return new MenuGlyphCellRenderer( control, this );
		}
	}

	internal class MenuGlyphCellRenderer: GridStaticCellRenderer
	{
		#region Constants
		/// <summary>
		/// Default glyph height.
		/// </summary>
		private const int c_iGlyphHeight = 7;
		/// <summary>
		/// Default glyph width.
		/// </summary>
		private const int c_iGlyphWidth = 4;
		#endregion

		public MenuGlyphCellRenderer( GridControlBase grid, GridCellModelBase model )
			: base( grid, model )
		{
		}

		protected override void DrawBackground( Graphics g, Rectangle rect, GridStyleInfo style, bool fillBackground )
		{
			MenuGridControlBase grid = Grid as MenuGridControlBase;
			if( grid != null && ( grid.ThemesEnabled && grid.IsVistaOS ) )
			{
				//Need this to avoid painting background over rounded borders
				//of this cell, which are drawing before filling background.
				rect.Inflate( 0, -2 );
				rect.Width -= 2;
			}

			base.DrawBackground( g, rect, style, fillBackground );
		}

		protected override void OnDraw( Graphics g, Rectangle rect, int rowIndex, int colIndex, GridStyleInfo style )
		{
			if( style.CellValue == null )
				return;

			if( (int)style.CellValue > 0 )
			{
				MenuGridControlBase grid = Grid as MenuGridControlBase;

				// Determine if selected or not
				bool selected = false;
				if( grid != null && grid.SelectedIndex != -1
					&& grid.SelectedIndex == rowIndex - 1 )
					selected = true;

				BarItem barItem = grid.parentItem.Items[rowIndex - 1];

				if( barItem.ReiseBeforePopupItemPaint( g, rect, ref selected, DrawElement.Glyph, style ) )
				{
					return;
				}

				if( grid.Style == VisualStyle.Office2007 && !( grid.ThemesEnabled && grid.IsVistaOS ) )
				{
					bool bRTL = this.Grid.IsRightToLeft();

					// draw with Office2007 visual style
					if( grid.IsItemDropDownStyle( barItem ) )
					{
						// Draw DropDownBarItem for menu
						Office2007MenuPainter.DrawMenuDropDownBarItem( g, rect, selected, bRTL );
					}
					else
					{
						// Draw ParentBarItem for menu
						Office2007MenuPainter.DrawMenuParentBarItem( g, rect, bRTL );
					}
				}
                else if (grid.Style == VisualStyle.Office2010 && !(grid.ThemesEnabled && grid.IsVistaOS))
                {
                    bool bRTL = this.Grid.IsRightToLeft();

                    // draw with Office2010 visual style
                    if (grid.IsItemDropDownStyle(barItem))
                    {
                        // Draw DropDownBarItem for menu
                        Office2010MenuPainter.DrawMenuDropDownBarItem(g, rect, selected, bRTL);
                    }
                    else
                    {
                        // Draw ParentBarItem for menu
                        Office2010MenuPainter.DrawMenuParentBarItem(g, rect, bRTL);
                    }
                }
                else if (grid.Style == VisualStyle.Metro)
                {  
                    bool bRTL = this.Grid.IsRightToLeft();

                    // draw with Metro visual style
                    if (grid.IsItemDropDownStyle(barItem))
                    {
                        // Draw DropDownBarItem for menu
                        MetroMenuPainter.DrawMenuDropDownBarItem(g, rect, selected, bRTL );
                    }
                    else
                    {
                        MetroMenuPainter.DrawMenuParentBarItem(g, rect, bRTL);
                    }
                }
                else
                {
                    // Draw a separator line if DropDown style
                    if (grid.IsItemDropDownStyle(barItem))
                    {
                        Pen linePen;
                        if (grid.ThemesEnabled && grid.IsVistaOS)
                        {
                            linePen = (selected) ? new Pen(VistaMenuColors.SelBorderColor) : new Pen(VistaMenuColors.BorderColor);
                        }
                        else
                        {
                            linePen = (selected) ? GetLinePen(grid.Style) :
                                new Pen(Color.FromArgb(75, SystemColors.ControlDarkDark));
                        }
                        if (this.Grid.IsRightToLeft())
                        {
                            g.DrawLine(linePen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                        }
                        else
                        {
                            g.DrawLine(linePen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                        }

						linePen.Dispose();
					}

					this.DrawGlyph( g, rect, barItem.Enabled, style );
				}

				barItem.ReiseAfterPopupItemPaint( g, rect, selected, DrawElement.Glyph, style );
			}
		}
		private void DrawGlyph( Graphics g, Rectangle rect, bool enabled, GridStyleInfo style )
		{
			// Draw the Arrow

			int height = c_iGlyphHeight;
			int width = c_iGlyphWidth;

			Rectangle drawRect = new Rectangle( rect.Left + ( rect.Width - width ) / 2,
				rect.Top + ( rect.Height - height ) / 2, width, height );

			Pen pen = new Pen( style.TextColor );

			int top = drawRect.Top;
			int left = drawRect.Left;
			Point ptTop,ptBottom;
			int PADDING = 7;
			MenuGridControlBase grid = Grid as MenuGridControlBase;
			if (grid != null && grid.parentItem.ResizeGlyphToFit)
			{
				ptTop = new Point(rect.Left, rect.Top + PADDING);
				ptBottom = new Point(rect.Left, rect.Bottom - PADDING);
			}
			else
			{
				int bottom = top + height + 1;
				ptTop = new Point(left + 1, top + 1);
				ptBottom = new Point(left + 1, bottom);
			}

			bool bRTL = this.Grid.IsRightToLeft();
			Matrix matrixPrev = null;

			if( bRTL )
			{
				matrixPrev = g.Transform;
				g.Transform = new Matrix( -1, 0, 0, 1, 2 * left + drawRect.Width, 0 );
			}

			while( ptTop.Y <= ptBottom.Y )
			{
				g.DrawLine( pen, ptTop, ptBottom );

				++ptTop.X;
				++ptTop.Y;
				++ptBottom.X;
				--ptBottom.Y;
			}

			if( bRTL )
			{
				g.Transform = matrixPrev;
			}

			//			g.DrawLine(pen, new Point(left+1, top + 1), new Point(left+1, top + 8));
			//			g.DrawLine(pen, new Point(left+2, top + 2), new Point(left+2, top + 7));
			//			g.DrawLine(pen, new Point(left+3, top + 3), new Point(left+3, top + 6));
			//			g.DrawLine(pen, new Point(left+4, top + 4), new Point(left+4, top + 5));
			pen.Dispose();
		}


		/// <summary>
		/// Gets pen for separator line amenably with VisualStyle.
		/// </summary>
		private Pen GetLinePen( VisualStyle style )
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
					pen = new Pen( Office2007OutlookColors.MenuSelectedItemBorderColor );
					break;
				}
				case VisualStyle.VS2005:
				{
					pen = new Pen( VS2005Colors.MenuSelectedItemBorderColor );
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

	}
	internal interface IMouseLeaveHandler
	{
		void OnMouseLeave();
		void HandlerDisconnected();
	}
	internal class CheckMarkCellModel: GridStaticCellModel
	{
		public CheckMarkCellModel( GridModel grid )
			: base( grid )
		{
			this.AllowFloating = true;
		}

		public override GridCellRendererBase CreateRenderer( GridControlBase control )
		{
			return new CheckMarkCellRenderer( control, this );
		}
	}

	internal class CheckMarkCellRenderer: GridStaticCellRenderer
	{
		public CheckMarkCellRenderer( GridControlBase grid, GridCellModelBase model )
			: base( grid, model )
		{
		}

		protected override void OnDraw( Graphics g, Rectangle rect, int rowIndex, int colIndex, GridStyleInfo style )
		{
			if( style.CellValue == null || (int)style.CellValue <= 0 )
				return;

			bool selected = false;
			MenuGridControlBase grid = Grid as MenuGridControlBase;
			if( grid != null )
			{
				selected = grid.HighlightRange.Top == rowIndex;
			}

			BarItem barItem = grid.parentItem.Items[rowIndex - 1];

			if( barItem.ReiseBeforePopupItemPaint( g, rect, ref selected, DrawElement.CheckMark, style ) )
			{
				return;
			}

			int width = 18, height = 18;
			Rectangle bounds = new Rectangle( rect.Left + 2,
				rect.Top + ( rect.Height - height ) / 2, width, height );

			// Interior
			SolidBrush br = new SolidBrush( Color.FromArgb( 70, MenuColors.SelColor ) );
			g.FillRectangle( br, bounds );
			br.Dispose();

			// CheckMark
			Pen pen = new Pen( SystemColors.ControlText );
			g.DrawLine( pen, bounds.Left + 5, bounds.Top + 8, bounds.Left + 7, bounds.Top + 10 );
			g.DrawLine( pen, bounds.Left + 5, bounds.Top + 9, bounds.Left + 7, bounds.Top + 11 );
			g.DrawLine( pen, bounds.Left + 7, bounds.Top + 10, bounds.Left + 11, bounds.Top + 6 );
			g.DrawLine( pen, bounds.Left + 7, bounds.Top + 11, bounds.Left + 11, bounds.Top + 7 );
			pen.Dispose();

			// Border
			bounds.Width--;
			bounds.Height--;
			pen = new Pen( MenuColors.SelBorderColor );
			g.DrawRectangle( pen, bounds );
			pen.Dispose();

			barItem.ReiseAfterPopupItemPaint( g, rect, selected, DrawElement.CheckMark, style );
		}
	}
	internal class SeparatorCellModel: GridStaticCellModel
	{
		public SeparatorCellModel( GridModel grid )
			: base( grid )
		{
		}

		public override GridCellRendererBase CreateRenderer( GridControlBase control )
		{
			return new SeparatorCellRenderer( control, this );
		}
	}

	internal class SeparatorCellRenderer: GridStaticCellRenderer
	{
		public SeparatorCellRenderer( GridControlBase grid, GridCellModelBase model )
			: base( grid, model )
		{
		}

		protected override void OnDraw( Graphics g, Rectangle rect, int rowIndex, int colIndex, GridStyleInfo style )
		{
			MenuGridControlBase grid = Grid as MenuGridControlBase;

			bool selected = false;

			if( grid != null )
			{
				selected = grid.HighlightRange.Top == rowIndex;
			}

			BarItem barItem = grid.parentItem.Items[rowIndex - 1];

			if( barItem.ReiseBeforePopupItemPaint( g, rect, ref selected, DrawElement.Separator, style ) )
			{
				return;
			}

			Pen pen;
			if( grid.ThemesEnabled && grid.IsVistaOS )
			{
				pen = new Pen( VistaMenuColors.SeparatorColor );
			}
			else
			{
				pen = GetSeparatorPen( grid.Style );
			}

			pen.Width = 1.0f;

			// X Left and Right are adjusted to make it draw right below the text.
			Point ptFrom, ptTo;

			if( grid.IsRightToLeft() )
			{
				ptFrom = new Point( rect.X, rect.Y + ( rect.Height / 2 ) );
				ptTo = new Point( rect.Right - 7, rect.Y + ( rect.Height / 2 ) );
			}
			else
			{
				ptFrom = new Point( rect.X + 7, rect.Y + ( rect.Height / 2 ) );
				ptTo = new Point( rect.Right, rect.Y + ( rect.Height / 2 ) );
			}

			g.DrawLine( pen, ptFrom, ptTo );

			if( grid.ThemesEnabled && grid.IsVistaOS )
			{
				ptFrom.Y++;
				ptTo.Y++;

				g.DrawLine( new Pen( Color.White ), ptFrom, ptTo );
			}

			pen.Dispose();

			barItem.ReiseAfterPopupItemPaint( g, rect, selected, DrawElement.Separator, style );
		}

		/// <summary>
		/// Gets pen for separator amenably with VisualStyle.
		/// </summary>
		private Pen GetSeparatorPen( VisualStyle style )
		{
			Pen separatorPen = null;
			MenuGridControlBase grid = Grid as MenuGridControlBase;

			if( grid != null && ( grid.ThemesEnabled && grid.IsVistaOS ) )
			{
				separatorPen = new Pen( VistaMenuColors.SeparatorColor );
			}
			else
			{
				switch( style )
				{
					case VisualStyle.Office2003:
					{
						separatorPen = new Pen( Office2003Colors.SeparatorColor );
						break;
					}

					case VisualStyle.Office2007Outlook:
					{
						separatorPen = new Pen( Office2007OutlookColors.MenuSeparatorColor );
						break;
					}

					case VisualStyle.VS2005:
					{
						separatorPen = new Pen( VS2005Colors.MenuSeparatorColor );
						break;
					}

					case VisualStyle.Office2007:
					{
						separatorPen = new Pen( Office2007Colors.Default.MenuSeparatorColor );
						break;
					}
                    case VisualStyle.Office2010:
                    {
                        separatorPen = new Pen(Office2010Colors.Default.MenuSeparatorColor);
                        break;
                    }

					default:
					{
						separatorPen = new Pen( Color.FromArgb( 75, SystemColors.ControlDarkDark ) );
						break;
					}
				}
			}

			return separatorPen;
		}
	}

	internal class IconCellModel: GridStaticCellModel
	{
		public IconCellModel( GridModel grid )
			: base( grid )
		{
			this.AllowFloating = true;
		}

		public override GridCellRendererBase CreateRenderer( GridControlBase control )
		{
			return new IconCellRenderer( control, this );
		}

		public override bool OnQueryCanFloatCell( int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style, Syncfusion.Windows.Forms.Grid.GridQueryFloatCell query )
		{
			return false;
		}

	}

	internal class IconCellRenderer: GridStaticCellRenderer
	{
		public IconCellRenderer( GridControlBase grid, GridCellModelBase model )
			: base( grid, model )
		{
		}

		protected virtual bool ShouldDrawImage( BarItem barItem )
		{
			if( barItem != null && barItem.PaintStyle != PaintStyle.TextOnly
				&& barItem.PaintStyle != PaintStyle.TextOnlyInMenus )
				return true;
			else
				return false;
		}

		private Rectangle GetIconRect( Rectangle cellRect, Size szIcon )
		{
			Rectangle drawRect = new Rectangle( cellRect.X + ( cellRect.Width - szIcon.Width ) / 2,
				cellRect.Y + ( cellRect.Height - szIcon.Height ) / 2,
				szIcon.Width, szIcon.Height );

			return drawRect;
		}

		protected override void DrawBackground( Graphics g, Rectangle rect, GridStyleInfo style, bool fillBackground )
		{
			MenuGridControlBase grid = Grid as MenuGridControlBase;
			if( grid != null && ( grid.ThemesEnabled && grid.IsVistaOS ) )
			{
				//Need this to avoid painting background over rounded borders
				//of this cell, which are drawing before filling background.
				rect.Offset( 2, 0 );
				rect.Inflate( 0, -2 );
			}

			base.DrawBackground( g, rect, style, fillBackground );
		}

		protected override void OnDraw( Graphics g, Rectangle rect, int rowIndex, int colIndex, GridStyleInfo style )
		{
			int imageIndex = style.ImageIndex;
			Rectangle savedRect = rect;

			// Determine if selected or not
			bool selected = false;
			bool bRTL = false;

			MenuGridControlBase grid = Grid as MenuGridControlBase;
			if( grid != null )
			{
				selected = grid.HighlightRange.Top == rowIndex;
				bRTL = grid.IsRightToLeft();
			}

			BarItem barItem = grid.parentItem.Items[rowIndex - 1];

			if( barItem.ReiseBeforePopupItemPaint( g, rect, ref selected, DrawElement.Icon, style ) )
			{
				return;
			}

			bool drawChecked = style.CellValue != null && (int)style.CellValue > 0;

			if( drawChecked )
			{
				int width = SystemInformation.MenuCheckSize.Width + 7,
						height = SystemInformation.MenuCheckSize.Height + 7;

				if( width > ( rect.Width - 1 ) )
					width = rect.Width - 1;

				if( height > ( rect.Height - 2 ) )
					height = rect.Height - 2;
                Rectangle bounds;
                bool bDrawCheckmark;
                if (!grid.parentItem.OverlapCheckBoxImageBounds)
                {
                    bounds = this.GetIconRect(new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - width, height), new Size(width, height));
                    if (barItem.Image == null && imageIndex < 0)
                        bounds = this.GetIconRect(rect, new Size(width, height));
                    bDrawCheckmark = true;
                }
                else
                {
                    bounds = this.GetIconRect(rect, new Size(width, height));
                    bDrawCheckmark = (barItem.Image == null && imageIndex < 0);
                }

				if( grid != null && ( grid.ThemesEnabled && grid.IsVistaOS ) )
				{
					if( selected )
					{
						SolidBrush fillBrush;
						if( bDrawCheckmark )
							fillBrush = new SolidBrush( VistaMenuColors.SelCheckedMenuBGColor );
						else
							fillBrush = new SolidBrush( VistaMenuColors.SelImageBGColor );

						Rectangle fRect = rect;
						fRect.Inflate( -1, -1 );

						g.FillRectangle( fillBrush, fRect );
						fillBrush.Dispose();
					}

					if( bDrawCheckmark )
					{
						Pen checkPen = new Pen( VistaMenuColors.SelCheckedMenuColor, 2 );
						Rectangle checkRect = bounds;
						checkRect.Inflate( -bounds.Width / 3, -bounds.Height / 4 );

						Point[] points = new Point[] {
                                                    new Point( checkRect.X, checkRect.Y + checkRect.Height / 2 ),
                                                    new Point( checkRect.X + checkRect.Width / 2 - 1, checkRect.Bottom - 2 ),
                                                    new Point( checkRect.Right - 1, checkRect.Top -1 )
                                                    };

						SmoothingMode prevMode = g.SmoothingMode;

						g.SmoothingMode = SmoothingMode.AntiAlias;
						g.DrawLines( checkPen, points );
						g.SmoothingMode = prevMode;

						checkPen.Dispose();
					}

					// Border
					Pen bPen;
					if( bDrawCheckmark )
						bPen = new Pen( VistaMenuColors.SelCheckedMenuBorderColor );
					else
						bPen = new Pen( VistaMenuColors.SelImageBorderColor );

					Rectangle bRect = rect;
					bRect.Width -= 2;
					bRect.Height--;

					g.DrawPath( bPen, DrawingUtils.GetRoundedRectangle( bRect, MenuGridControlBase.DEF_BORDERRADIUS ) );
				}
				else if( grid.Style == VisualStyle.Office2007 )
				{
                    if (!grid.parentItem.OverlapCheckBoxImageBounds)
                        Office2007MenuPainter.DrawMenuChecked(g, bounds, bDrawCheckmark);
                    else
                        Office2007MenuPainter.DrawMenuChecked(g, rect, bDrawCheckmark);
				}
                else if (grid.Style == VisualStyle.Office2010)
                {
                    if (!grid.parentItem.OverlapCheckBoxImageBounds)
                        Office2010MenuPainter.DrawMenuChecked(g, bounds, bDrawCheckmark);
                    else
                        Office2010MenuPainter.DrawMenuChecked(g, rect, bDrawCheckmark);
                }
                else if (grid.Style == VisualStyle.Metro)
                {
                    if (!grid.parentItem.OverlapCheckBoxImageBounds)
                        MetroMenuPainter.DrawMenuChecked(g, bounds, bDrawCheckmark);
                    else
                        MetroMenuPainter.DrawMenuChecked(g, rect, bDrawCheckmark);
                }
                else
                {
                    bounds.X += 1;

					// Interior
					SolidBrush br = ( selected ) ?
						new SolidBrush( GetSelectedCheckMarkColor( grid.Style ) ) :
						new SolidBrush( GetCheckMarkColor( grid.Style ) );
					g.FillRectangle( br, bounds );
					br.Dispose();

					// Only if an image is not available draw the check mark.
					if( bDrawCheckmark )
					{
						Pen pen = new Pen( SystemColors.ControlText );

						// CheckMark
						int left1 = (int)( (float)bounds.Width / 4f + (float)bounds.Width / 20f );
						int left2 = left1 + (int)( (float)bounds.Width / 10f );
						int left3 = left2 + (int)( (float)bounds.Width / 5f );

						int top1 = (int)( (float)bounds.Height / 4f + (float)bounds.Height / 5f );
						int top2 = top1 + (int)( left2 - left1 );
						int top3 = top2 - (int)( left3 - left2 );

						int thickness = (int)( (float)bounds.Width / 10f );

						for( int i = 0; i < thickness; i++ )
						{
							g.DrawLine( pen, bounds.Left + left1, bounds.Top + top1 + i, bounds.Left + left2, bounds.Top + top2 + i );
							g.DrawLine( pen, bounds.Left + left2, bounds.Top + top2 + i, bounds.Left + left3, bounds.Top + top3 + i );
						}

						pen.Dispose();
					}

					// Border
					bounds.Width--;
					bounds.Height--;
					Pen pen1 = GetCheckMarkBorderPen( grid.Style, selected );
					g.DrawRectangle( pen1, bounds );
					pen1.Dispose();
				}
			}

			if( grid.ThemesEnabled && grid.IsVistaOS )
			{
				Pen pen;
				if( selected )
					pen = new Pen( Color.FromArgb( 64, VistaMenuColors.SeparatorColor ) );
				else
					pen = new Pen( VistaMenuColors.SeparatorColor );

				if( grid.IsRightToLeft() )
				{
					Point ptFrom = new Point( savedRect.Left + 1, savedRect.Top );
					Point ptTo = new Point( savedRect.Left + 1, savedRect.Bottom );

					g.DrawLine( new Pen( Color.FromArgb( 64, VistaMenuColors.SelBorderColor ) ), ptFrom, ptTo );
					g.DrawLine( pen, ptFrom, ptTo );

					ptFrom.Offset( -1, 1 );
					ptTo.Offset( -1, -2 );

					g.DrawLine( new Pen( VistaMenuColors.BackgroundColor ), ptFrom, ptTo );
				}
				else
				{
					Point ptFrom = new Point( savedRect.Right - 1, savedRect.Top );
					Point ptTo = new Point( savedRect.Right - 1, savedRect.Bottom );

					g.DrawLine( new Pen( Color.FromArgb( 64, VistaMenuColors.SelBorderColor ) ), ptFrom, ptTo );
					g.DrawLine( pen, ptFrom, ptTo );

					ptFrom.Offset( 1, 1 );
					ptTo.Offset( 1, -2 );

					g.DrawLine( new Pen( VistaMenuColors.BackgroundColor ), ptFrom, ptTo );
				}

				pen.Dispose();
			}

			// Get ImageIndex and Icon
			if( !this.ShouldDrawImage( barItem ) )
				return;

			bool mouseDown = false;
			
			if( grid.IsItemDropDownStyle( barItem ) && grid is MenuGrid )
			{
				MenuGrid menuGrid = grid as MenuGrid;

				if (!menuGrid.Customizing && Control.MouseButtons == MouseButtons.Left && menuGrid.currentVisibleChildIndex == -1)
				{
					mouseDown = true;
				}
			}

			Size szImage = barItem.GetImageSizeInternal(false);
			
            Rectangle drawRect;
            if (!grid.parentItem.OverlapCheckBoxImageBounds)
                drawRect = this.GetIconRect(new Rectangle(rect.X + (SystemInformation.MenuCheckSize.Width + 7), rect.Y, rect.Width - (SystemInformation.MenuCheckSize.Width + 7), rect.Height), szImage.IsEmpty ? BarItem.DEF_IMAGE_SIZE : szImage);
            else
                drawRect = this.GetIconRect(rect, szImage.IsEmpty ? BarItem.DEF_IMAGE_SIZE : szImage);
            if(!drawChecked)
                drawRect = this.GetIconRect(rect, szImage.IsEmpty ? BarItem.DEF_IMAGE_SIZE : szImage);
			Rectangle imageBorder = Rectangle.Empty;

			if( !( grid.ThemesEnabled && grid.IsVistaOS ) && selected && style.Enabled && ( mouseDown || drawChecked ) )
			{
				// MouseDown background around image
				// int width = 18, height = 18;
				//imageBorder = new Rectangle(rect.Left + 2,
				//	rect.Top + (rect.Height - height)/2, width, height);
				imageBorder = rect;

				// Interior
				SolidBrush br = null;
				if( grid.Style != VisualStyle.Office2003 )
					br = new SolidBrush( Color.FromArgb( 70, MenuColors.SelBorderColor ) );
				else
					br = new SolidBrush( Office2003Colors.CheckedSelColor );
                int index = barItem.ImageIndex;
                IList images = barItem.GetImageListInternal(false);
                if (images != null)
                {
                    if ((index >= 0 && index < images.Count) || barItem.Image != null)
                    {
                        g.FillRectangle(br, imageBorder);
                    }
                }
				br.Dispose();
			}

			// Image
			if( !style.Enabled )
			{
				if( selected && !drawChecked && 
					grid.Style != VisualStyle.Office2003
					&& grid.Style != VisualStyle.VS2005
					&& grid.Style != VisualStyle.Office2007
                    && grid.Style != VisualStyle.Office2010
					&& grid.Style != VisualStyle.Office2007Outlook )
					drawRect.Offset( ( bRTL ? 0 : 1 ), 1 );

				if (!DrawDisabledImage(g, drawRect, barItem))
				{
					int idx = barItem.ImageIndex;
					IList images = barItem.GetImageListInternal(false);

					if (images != null && idx >= 0 && idx < images.Count)
					{
						Image image = images[idx] as Image;

						Color bgColor = grid.Style != VisualStyle.Office2003 ? MenuColors.SelColor : Office2003Colors.CheckedColor;
						ControlPaint.DrawImageDisabled(g, image, drawRect.Left, drawRect.Top, bgColor);
					}
					else if (barItem.Image != null)
					{
						barItem.Image.Draw(g, drawRect, DrawItemState.Disabled);
					}
				}
			}
			else
			{
				Image _image = null;
				ImageExt _imageExt = null;

				if (selected)
				{
					int idx = barItem.HighlightedImageIndex;
					IList images = barItem.GetHighlightImageListInternal(false);

					if (images != null && idx >= 0 && idx < images.Count)
					{
						_image = images[idx] as Image;
					}
					else
					{
						_imageExt = barItem.HighlightedImage;
					}
				}

				if (_image == null && _imageExt == null)
				{
					int idx = barItem.ImageIndex;
					IList images = barItem.GetImageListInternal(false);

					if (images != null && idx >= 0 && idx < images.Count)
					{
						_image = images[idx] as Image;
					}
					else
					{
						_imageExt = barItem.Image;
					}
				}

				if (_image != null || _imageExt != null)
				{
					bool _disposeImage = false;

					if (selected && !mouseDown && !drawChecked
						&& grid.Style != VisualStyle.Office2003
						&& grid.Style != VisualStyle.VS2005
						&& grid.Style != VisualStyle.Office2007
                        && grid.Style != VisualStyle.Office2010
						&& grid.Style != VisualStyle.Office2007Outlook
						&& !(grid.ThemesEnabled && grid.IsVistaOS))
					{
						if (_image == null)
						{
							_image = _imageExt.GetImage();
							_disposeImage = true;
						}
						using (Image bmp = new Bitmap(_image, drawRect.Size))
						{
							DrawingUtils.DrawShadow(g, bmp, drawRect.Left, drawRect.Top);
						}
						drawRect.Offset((bRTL ? 1 : -1), -1);
					}

					if (_imageExt != null)
					{
						_imageExt.Draw(g, drawRect, DrawItemState.None);
					}
					else
					{
						DrawingUtils.DrawImage(g, _image, drawRect.Left, drawRect.Top, drawRect.Width, drawRect.Height);
					}

					if (_disposeImage)
					{
						_image.Dispose();
					}
				}

				if( !selected )
				{
					if( !grid.ThemesEnabled
						&& grid.Style != VisualStyle.Office2003
						&& grid.Style != VisualStyle.VS2005
						&& grid.Style != VisualStyle.Office2007
                        && grid.Style != VisualStyle.Office2010
						&& grid.Style != VisualStyle.Office2007Outlook
						&& !( grid.ThemesEnabled && grid.IsVistaOS )
                        &&  grid.Style != VisualStyle.Metro)
					{
						SolidBrush br = new SolidBrush( Color.FromArgb( 255 - MenuColors.InactiveItemAlphaBlendFactor, style.Interior.BackColor ) );
						g.FillRectangle( br, drawRect );
						br.Dispose();
					}
				}
			}

			if( !( grid.ThemesEnabled && grid.IsVistaOS ) && selected && style.Enabled && mouseDown )
			{
				imageBorder.Width--;
				imageBorder.Height--;
				Color borderColor = grid.Style != VisualStyle.Office2003 ? MenuColors.SelBorderColor : Office2003Colors.SelBorderColor;
				Pen pen = new Pen( borderColor );
                int index = barItem.ImageIndex;
                IList images = barItem.GetImageListInternal(false);
                if ((index >= 0 && index < images.Count) || barItem.Image != null)
                {
                    g.DrawRectangle(pen, imageBorder);
                }
				pen.Dispose();
			}

			barItem.ReiseAfterPopupItemPaint( g, rect, selected, DrawElement.Icon, style );
		}

		private bool DrawDisabledImage(Graphics g, Rectangle rc, BarItem barItem)
		{
			int disabledIdx = barItem.DisabledImageIndex;
			IList disabledImages = barItem.GetDisabledImageListInternal(false);

			if (disabledImages != null && disabledIdx >= 0 && disabledIdx < disabledImages.Count)
			{
				Image image = disabledImages[disabledIdx] as Image;
				DrawingUtils.DrawImage(g, image, rc.Left, rc.Top, rc.Width, rc.Height);
				return true;
			}

			if (barItem.DisabledImage != null)
			{
				barItem.DisabledImage.Draw(g, rc, DrawItemState.None);
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// Gets pen for border of the check mark amenably with VisualStyle.
		/// </summary>
		private Pen GetCheckMarkBorderPen( VisualStyle style, bool bSelected )
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
					pen = ( bSelected ) ?
					new Pen( Office2007OutlookColors.MenuSelectedCheckMarkBorderColor ) :
					new Pen( Office2007OutlookColors.MenuCheckMarkBorderColor );
					break;
				}
				case VisualStyle.VS2005:
				{
					pen = new Pen( VS2005Colors.MenuSelectedItemBorderColor );
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
		/// Gets color of the check mark amenably with VisualStyle.
		/// </summary>
		private Color GetCheckMarkColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = Office2003Colors.CheckedColor;
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					color = Office2007OutlookColors.MenuCheckMarkColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = VS2005Colors.MenuCheckMarkColor;
					break;
				}
				default:
				{
					color = Color.FromArgb( 70, MenuColors.SelColor );
					break;
				}
			}

			return color;
		}


		/// <summary>
		/// Gets color of the selected check mark amenably with VisualStyle.
		/// </summary>
		private Color GetSelectedCheckMarkColor( VisualStyle style )
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
					color = Office2007OutlookColors.MenuSelectedCheckMarkColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = VS2005Colors.MenuSelectedCheckMarkColor;
					break;
				}
				default:
				{
					color = Color.FromArgb( 70, MenuColors.SelColor );
					break;
				}
			}

			return color;
		}

	}
}
