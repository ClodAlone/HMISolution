#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Syncfusion.Runtime.InteropServices;

#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Summary description for VirtualListView.
	/// </summary>
	public class VirtualListView: DataListView
	{
		#region Constants
		private const int DEF_TEXTSPAN = 2;
		#endregion

		#region Members
		private DataView m_view = null;
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public int SelectedItemsCount
		{
			get
			{
				IntPtr result = NativeMethods.SendMessage( Handle, NativeMethods.LVM_GETSELECTEDCOUNT,
					IntPtr.Zero, IntPtr.Zero );

				return result.ToInt32();
			}
		}
		/// <summary>
		/// Gets/sets the sets the number of items to be displayed.
		/// </summary>
		[
		Category( "Behavior" ),
		DefaultValue( 0 )
		]
		public int ItemsCount
		{
			get
			{
				return this.VirtualListSize;
			}
			set
			{
				this.VirtualListSize = value;
			}
		}
		/// <summary>
		/// Gets the value whether horizontal scrollbar is visible.
		/// </summary>
		protected bool IsHorizontalScrollBarVisible
		{
			get
			{
				if( !this.IsHandleCreated )
				{
					return false;
				}

				return ( NativeMethods.GetWindowLong( this.Handle, NativeMethods.GWL_STYLE ) & NativeMethods.WS_HSCROLL ) != 0;
			}
		}

		/// <summary>
		/// Gets the value whether vertical scrollbar is visible.
		/// </summary>
		protected bool IsVerticalScrollBarVisible
		{
			get
			{
				if( !this.IsHandleCreated )
				{
					return false;
				}

				return ( NativeMethods.GetWindowLong( this.Handle, NativeMethods.GWL_STYLE ) & NativeMethods.WS_VSCROLL ) != 0;
			}
		}

		#endregion

		#region Events
		public event QueryItemTextHandler QueryItemText;
		#endregion

		#region Initialization
		public VirtualListView()
		{
			this.SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true );

			this.OwnerDraw = true;
			this.VirtualMode = true;
			this.View = View.Details;
			this.Sorting = SortOrder.None;
		}
		#endregion

		#region Public methods
		public virtual string GetSelectedItemText()
		{
			string strText = string.Empty;

			if( m_view != null && m_view.Count > 0 && m_view.Table.Columns.Count > 0 )
			{
				int selIndex = GetSelectedIndex();

				if( selIndex >= 0 )
				{
					strText = m_view[selIndex].Row[0].ToString();
				}
			}

			return strText;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual int GetSelectedIndex()
		{
			int result = (int)NativeMethods.SendMessage( Handle, NativeMethods.LVM_GETNEXTITEM, -1,
				new IntPtr( NativeMethods.LVNI_SELECTED ) );

			return result;
		}

		public virtual void SetSelectedIndex( int index )
		{
			if( this.IsHandleCreated )
			{
				if( index >= 0 )
				{
					SetSelectedIndex( index, true );
				}
				else
				{
					index = GetSelectedIndex();

					if( index >= 0 )
					{
						SetSelectedIndex( index, false );
					}
				}
			}
		}

		private void SetSelectedIndex( int index, bool select )
		{
			NativeMethods.LVITEM item = new NativeMethods.LVITEM();

			item.mask = NativeMethods.LVIF_STATE;
			item.state = select ? NativeMethods.LVIS_SELECTED : 0;
			item.stateMask = NativeMethods.LVIS_SELECTED;

			IntPtr pItem = IntPtr.Zero;

			try
			{
				pItem = Marshal.AllocHGlobal( Marshal.SizeOf( item ) );
				Marshal.StructureToPtr( item, pItem, true );

				NativeMethods.SendMessage( this.Handle, NativeMethods.LVM_SETITEMSTATE, index, pItem );
			}
			finally
			{
				Marshal.FreeHGlobal( pItem );
			}
		}

		public int GetHotIndex()
		{
			return (int)NativeMethods.SendMessage( Handle, NativeMethods.LVM_GETHOTITEM, 0, 0 );
		}

		public bool IsItemSelected( int index )
		{
			IntPtr result = NativeMethods.SendMessage( Handle, NativeMethods.LVM_GETNEXTITEM,
				new IntPtr( index ),
				new IntPtr( (int)NativeMethods.LVIS_SELECTED ) );

			return ( result != IntPtr.Zero );
		}
		/// <summary>
		/// Set the state of the passed Listview item's index.
		/// </summary>
		/// <param name="index">Listview item's index.</param>
		/// <param name="selected">Select the passed item?</param>
		public void SelectItem( int index, bool selected )
		{
			if( index >= 0 && index < this.Items.Count )
			{
				ListViewItem item = this.Items[index];
				if( item != null )
				{
					item.Selected = selected;
					item.EnsureVisible();
				}
			}
		}
		#endregion

		#region Overrides
        protected override void Dispose(bool disposing)
        {
            if( disposing )
            {
                if( m_view != null )
                {
                    m_view.Dispose();
                    m_view = null;
                }
            }
            base.Dispose(disposing);
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="oldDataSource"></param>
		/// <param name="newDataSource"></param>
		protected override void OnDataSourceChanged( object oldDataSource, object newDataSource )
		{
			m_view = newDataSource as DataView;

			if( m_view != null )
			{
				ItemsCount = m_view.Count;
			}
		}

		/// <summary>
		/// Populates the ListView based on the columns in the
		/// DataSource.
		/// </summary>
		public override void PopulateList()
		{
		}

		/// <summary>
		/// Sets the control Height based on the item count subject
		/// to a maximum height.
		/// </summary>
		/// <param name="maxHeight">The maximum height.</param>
		public override void AdjustHeight( int maxHeight )
		{
			if( this.Items.Count > 0 || this.ItemsCount > 0 )
			{
				int adjustedMaxHeight = GetAdjustedMaxHeight( maxHeight );

				int totalHeight;
				int itemHeight = this.GetItemHeight();

				if( this.Items.Count > 0 )
					totalHeight = this.GetItemRect( 0 ).Height * ( this.Items.Count + 1 );
				else
					totalHeight = itemHeight * ( this.ItemsCount + 1 );

				if( this.HeaderStyle != ColumnHeaderStyle.None )
				{
					totalHeight += this.GetHeaderHeight();
				}

				if( totalHeight < adjustedMaxHeight )
					this.Height = totalHeight;
				else
					this.Height = adjustedMaxHeight;

				if( this.IsHorizontalScrollBarVisible )
				{
					this.Height += SystemInformation.HorizontalScrollBarHeight;
				}

				SizablePopupControlContainer container = this.Parent as SizablePopupControlContainer;

				if( container != null )
				{
					container.FitToChildControlSize = true;
				}
			}
		}

		/// <summary>
		/// Gets the height of the adjusted max.
		/// </summary>
		/// <param name="actualMaxHeight">Actual height of the max.</param>
		/// <returns></returns>
		protected override int GetAdjustedMaxHeight( int actualMaxHeight )
		{
			int itemsAllowed = 0;
			int maxHeight;
			int itemHeight = this.GetItemHeight();

			if( this.Items.Count > 0 )
				itemsAllowed = actualMaxHeight / this.GetItemRect( 0 ).Height;
			else if( this.ItemsCount > 0 )
				itemsAllowed = actualMaxHeight / itemHeight;

			if( this.Items.Count > 0 )
				maxHeight = itemsAllowed * this.GetItemRect( 0 ).Height;
			else
				maxHeight = itemsAllowed * itemHeight;

			return maxHeight;
		}

		private int GetHeaderHeight()
		{
			int height = 0;

			IntPtr headerPtr = NativeMethods.SendMessage( this.Handle, NativeMethods.LVM_GETHEADER, 0, 0 );
			if( headerPtr != IntPtr.Zero )
			{
				NativeMethods.RECT headerRect = new NativeMethods.RECT();
				NativeMethods.GetWindowRect( headerPtr.ToInt32(), ref headerRect );

				height += headerRect.Height;
			}

			return height;
		}

		private int GetItemHeight()
		{
			int height = NativeMethods.HIWORD( NativeMethods.SendMessage( this.Handle, NativeMethods.LVM_GETITEMSPACING, 1, 0 ) );

			if( this.SmallImageList != null && this.SmallImageList.ImageSize.Height > height )
			{
				height = this.SmallImageList.ImageSize.Height + 1;
			}

			return height;
		}

		/// <summary>
		/// Gets index of the item which is at a specified position.
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
		private int HitTest( Point pt )
#else
		private new int HitTest( Point pt )
#endif
		{
			NativeMethods.LV_HITTESTINFO hitTestInfo = new NativeMethods.LV_HITTESTINFO();
			hitTestInfo.pt = new NativeMethods.POINT( pt.X, pt.Y );

			IntPtr ptr = Marshal.AllocHGlobal( Marshal.SizeOf( typeof( NativeMethods.LV_HITTESTINFO ) ) );
			Marshal.StructureToPtr( hitTestInfo, ptr, true );

			IntPtr indx = NativeMethods.SendMessage( this.Handle, NativeMethods.LVM_HITTEST, 0, ptr );
			Marshal.FreeHGlobal( ptr );

			int index = indx.ToInt32();

			return index;
		}

		private Point GetItemPosition( int index )
		{
			IntPtr ptrPoint = Marshal.AllocHGlobal( Marshal.SizeOf( typeof( NativeMethods.POINT ) ) );

			NativeMethods.SendMessage( this.Handle, NativeMethods.LVM_GETITEMPOSITION, index, ptrPoint );
			NativeMethods.POINT nPoint = (NativeMethods.POINT)Marshal.PtrToStructure( ptrPoint, typeof( NativeMethods.POINT ) );

			Marshal.FreeHGlobal( ptrPoint );

			return new Point( nPoint.X, nPoint.Y );
		}

		/// <summary>
		/// Overrides <see cref="M:System.Windows.Forms.Control.WndProc(System.Windows.Forms.Message@)"/>.
		/// </summary>
		/// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"/> to process.</param>
		protected override void WndProc( ref Message m )
		{
			if( m.Msg == NativeMethods.WM_SETFOCUS )
			{
				base.DefWndProc( ref m );
				return;
			}
			base.WndProc( ref m );
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			if( this.Parent is PopupControlContainer )
			{
				PopupControlContainer popupControl = this.Parent as PopupControlContainer;

				popupControl.HidePopup( PopupCloseType.Done );
				popupControl.FocusParent();
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseMove( MouseEventArgs e )
		{
			int index = HitTest( new Point( e.X, e.Y ) );

			if( index != -1 )
			{
				this.SelectItem( index, true );
			}

			base.OnMouseMove( e );
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.ListView.DrawColumnHeader"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.DrawListViewColumnHeaderEventArgs"/> that contains the event data.</param>
		protected override void OnDrawColumnHeader( DrawListViewColumnHeaderEventArgs e )
		{
			e.DrawDefault = true;
		}

		/// <summary>
		/// Draw item's background
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.DrawListViewItemEventArgs"/> that contains the event data.</param>
		protected override void OnDrawItem( DrawListViewItemEventArgs e )
		{
			if( e.State != 0 )
			{
				ListViewItem item = e.Item;

				if( item.Selected )
				{
					int x = GetItemPosition( item.Index ).X;
					ImageList imageList = item.ImageList;
					int imgWidth = 0;

					if( imageList != null )
					{
						imgWidth = imageList.ImageSize.Width + DEF_TEXTSPAN;
						x += imgWidth;
					}

					int w = e.Bounds.Right - x;
					
					if (this.RightToLeft == RightToLeft.Yes)
					{
						x = this.ClientRectangle.Width - e.Bounds.Right;
					}
                    AutoComplete autoComplete = this.ListOwner as AutoComplete;
                    if (autoComplete != null && autoComplete.Style == AutoCompleteStyle.Metro)
                    {
                        using (SolidBrush brush = new SolidBrush(autoComplete.MetroColor))
                        {

                            e.Graphics.FillRectangle(brush, x - 2, e.Bounds.Top, w, e.Bounds.Height);
                        }
                    }
                    else
					e.Graphics.FillRectangle(SystemBrushes.Highlight, x - 2, e.Bounds.Top, w, e.Bounds.Height);
				}
				else
				{
					e.DrawBackground();
				}
			}
		}

		/// <summary>
		/// Draw image and subitems' text
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.DrawListViewSubItemEventArgs"/> that contains the event data.</param>
		protected override void OnDrawSubItem( DrawListViewSubItemEventArgs e )
		{
			if( e.ItemState != 0 )
			{
				ListViewItem item = e.Item;
				Rectangle rcItem = e.Bounds;

				if( e.ColumnIndex == 0 )
				{
					// Reserve space in the first column and draw image if necessary
					ImageList imageList = item.ImageList;
					if( imageList != null )
					{
						int x = GetItemPosition( item.Index ).X;

						int imageIndex = item.ImageIndex;

						if( imageIndex >= 0 && imageIndex < imageList.Images.Count )
						{
							imageList.Draw( e.Graphics, x, rcItem.Top, imageIndex );
						}

						int imgWidth = imageList.ImageSize.Width + DEF_TEXTSPAN;

						x += imgWidth;

						rcItem.X = x;
                        rcItem.Width -= imgWidth;
					}
				}

                TextFormatFlags tfDraw = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix;

				Color color = item.Selected ? SystemColors.HighlightText : item.ForeColor;
				Font font = item.Font;

				string text = e.SubItem.Text;

				AutoComplete autoComplete = this.ListOwner as AutoComplete;
				if( autoComplete != null && autoComplete.GetAutoComplete() == AutoCompleteModes.MultiSuggestExtended )
				{
					Control edit = autoComplete.GetActiveEditControl();
					if( edit != null )
					{
						string key = edit.Text;
						int len = key.Length;

						if( len>0 )
						{
							StringComparison strComp = autoComplete.CaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
							int pos = text.IndexOf( key, strComp );

							if( pos >= 0 )
							{
                                TextFormatFlags tfMeasure = TextFormatFlags.NoClipping | TextFormatFlags.NoPrefix;

								int spaceWidth1 = TextRenderer.MeasureText( " ", font, Size.Empty, tfMeasure ).Width;
								int spaceWidth2 = TextRenderer.MeasureText( "  ", font, Size.Empty, tfMeasure ).Width;

								int padding = spaceWidth1 - ( spaceWidth2 - spaceWidth1 );

								int x = rcItem.X;

								if( pos > 0 )
								{
									string str = text.Substring( 0, pos );

									TextRenderer.DrawText( e.Graphics, str, font, rcItem, color, tfDraw );

									x += TextRenderer.MeasureText( str, font, Size.Empty, tfMeasure ).Width;
								}

								if( x < rcItem.Right )
								{
									if( x > rcItem.X )
									{
										x -= padding;

										rcItem.Width = rcItem.Right - x;
										rcItem.X = x;
									}

									Font keyFont = new Font( font, FontStyle.Bold );
									string keyText = text.Substring( pos, len );

									TextRenderer.DrawText( e.Graphics, keyText, keyFont, rcItem, color, tfDraw );

									x += TextRenderer.MeasureText( keyText, keyFont, Size.Empty, tfMeasure ).Width;

									if( x < rcItem.Right )
									{
										pos += len;

										if( pos < text.Length )
										{
											x -= padding;

											rcItem.Width = rcItem.Right - x;
											rcItem.X = x;

											string str = text.Substring( pos );
											TextRenderer.DrawText( e.Graphics, str, font, rcItem, color, tfDraw );
										}
									}
								}

								return;
							}
						}
					}
				}

				TextRenderer.DrawText( e.Graphics, text, font, rcItem, color, tfDraw );
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.ListView.RetrieveVirtualItem"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.RetrieveVirtualItemEventArgs"/> that contains the event data.</param>
		protected override void OnRetrieveVirtualItem( RetrieveVirtualItemEventArgs e )
		{
			if( m_view != null )
			{
				int idx = e.ItemIndex;
				if( idx >= 0 && idx < ItemsCount )
				{
					string[] items = new string[this.Columns.Count];
					object[] values = m_view[idx].Row.ItemArray;

					int cols = Math.Min( values.Length, items.Length );

					int imageIdx = -1;
					int imageCol = this.ImageColumnIndex;

					for( int i = 0; i < cols; i++ )
					{
						string sItem = values[i].ToString();

						if( i == imageCol )
						{
							imageIdx = Convert.ToInt16( sItem );
						}
						else
						{
							if( QueryItemText != null )
							{
								QueryItemTextEventArgs qe = new QueryItemTextEventArgs( idx, i );

								qe.Text = sItem;

								QueryItemText( this, qe );

								sItem = qe.Text;
							}

							items[i] = sItem;
						}
					}

					e.Item = new ListViewItem( items, imageIdx );
				}
			}

			base.OnRetrieveVirtualItem( e );
		}


		#endregion
	}

	#region Delegates
	/// <summary>
	/// Retrieve the text for a ListView cell (item and subitem).
	/// </summary>
	public delegate void QueryItemTextHandler( object sender, QueryItemTextEventArgs e );

	public class QueryItemTextEventArgs: EventArgs
	{
		#region members
		private int m_item = -1;
		private int m_subItem = -1;
		private string m_strText = null;
		#endregion

		#region Properties
		/// <summary>
		/// Listview item (row).
		/// </summary>
		public int Item
		{
			get
			{
				return m_item;
			}
		}
		/// <summary>
		/// Listview subitem (column).
		/// </summary>
		public int SubItem
		{
			get
			{
				return m_subItem;
			}
		}
		/// <summary>
		/// Text to display.
		/// </summary>
		public string Text
		{
			get
			{
				return m_strText;
			}
			set
			{
				if( value != m_strText )
				{
					m_strText = value;
				}
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <param name="subItem"></param>
		public QueryItemTextEventArgs( int item, int subItem )
		{
			m_item = item;
			m_subItem = subItem;
		}
		#endregion
	}
	#endregion
}