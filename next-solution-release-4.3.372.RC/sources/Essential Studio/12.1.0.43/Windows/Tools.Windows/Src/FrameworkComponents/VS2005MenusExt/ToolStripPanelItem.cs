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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Runtime.InteropServices;
//Syncfusion
using Syncfusion.Windows.Forms.Tools.Design;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	#region ToolStripPanelItem
	[Designer( typeof( Design.ToolStripPanelItemDesigner ) )]
	[System.Windows.Forms.Design.ToolStripItemDesignerAvailability( System.Windows.Forms.Design.ToolStripItemDesignerAvailability.ToolStrip | System.Windows.Forms.Design.ToolStripItemDesignerAvailability.StatusStrip )]
	[ToolboxBitmap( typeof( Syncfusion.Windows.Forms.Tools.ToolStripPanelItem ), "ToolboxIcons.ToolStripPanelItem.bmp" )]
	public class ToolStripPanelItem: ToolStripControlHost
	{
		#region Constants
		const int DEF_ROWCOUNT = 3;
		const int DEF_PADDING = 2;
        const int DPI_150_Gap = 10;
        const int DPI_125_Gap = 5;
		#endregion

		#region *** ToolStripInternal
		internal class ToolStripInternal: ToolStripEx, INativeMessageFilter
		{
			#region Constants
			const SetWindowPosFlags SWP_MOVEFLAGS = SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOSIZE;
			#endregion

			#region Nested classes

			#region *** LayoutData
			class LayoutData
			{
				#region Constructors
				public LayoutData( ToolStripInternal ts )
				{
					ToolStripItemCollection items = ts.Items;
					int count = items.Count;
					if( count > 0 )
					{
						int maxRows = ts.RowCount;

						int nRows = Math.Min( maxRows, count );
						int nCols = ( count + maxRows - 1 ) / maxRows;

						m_aRows = new int[nRows];
						m_aCols = new int[nCols];

						m_nRows = 0;
						m_nCols = 0;

						int col = 0;
						int row = 0;
						int idx = 0;

						while( idx < count )
						{
							ToolStripItem item = items[idx++];

							if( item != null && item.Available )
							{
                                Size scalSize = item.GetPreferredSize(Size.Empty);
                                using (Graphics g = Graphics.FromImage( new Bitmap(10, 10)))
                                {
                                    if (m_nRows < row + 1)
                                    {
                                        m_nRows = row + 1;
                                    }

								if( m_nCols < col + 1 )
								{
									m_nCols = col + 1;
								}
                                    Size szItem = item.AutoSize ? item.GetPreferredSize(Size.Empty) : item.Size;
                                    if (g.DpiX > 120 && ts.DefaultDPISize)
                                    {
                                        if (item is ToolStripButton && (item as ToolStripButton).DisplayStyle == ToolStripItemDisplayStyle.Image)
                                        {
                                            scalSize = new Size(28, 28);
                                        }
                                        else if (item is ToolStripSplitButtonEx)
                                        {
                                            scalSize = new Size(40, 28);
                                        }
                                        else if (item is ToolStripSeparator)
                                        {
                                            scalSize = new Size(4, 32);
                                        }
                                        item.AutoSize = false;
                                        item.Size = scalSize;
                                        szItem = scalSize;
                                    }
                                    else if (g.DpiX > 96 && ts.DefaultDPISize)
                                    {
                                        if (item is ToolStripButton && (item as ToolStripButton).DisplayStyle == ToolStripItemDisplayStyle.Image)
                                        {
                                            scalSize = new Size(26, 26);
                                        }
                                        else if (item is ToolStripSplitButtonEx)
                                        {
                                            scalSize = new Size(35, 24);
                                        }
                                        else if (item is ToolStripSeparator)
                                        {
                                            scalSize = new Size(3, 26);
                                        }
                                        item.AutoSize = false;
                                        item.Size = scalSize;
                                        szItem = scalSize;
                                    }
                                    if (ts.Parent != null && ts.Parent.FindForm() != null)
                                    {
                                        foreach(Control ctrl in ts.Parent.FindForm().Controls)
                                        {
                                            if (ctrl is RibbonControlAdv)
                                            {
                                                if ((ctrl as RibbonControlAdv).RibbonTouchModeEnabled)
                                                {
                                                    if (item is ToolStripButton && (item as ToolStripButton).DisplayStyle == ToolStripItemDisplayStyle.Image)
                                                    {
                                                        scalSize = new Size(40, 28);
                                                    }
                                                    else if (item is ToolStripSplitButtonEx)
                                                    {
                                                        scalSize = new Size(46, 28);
                                                    }
                                                    else if (item is ToolStripSeparator)
                                                    {
                                                        scalSize = new Size(4, 32);
                                                    }
                                                    item.AutoSize = false;
                                                    item.Size = scalSize;
                                                    szItem = scalSize;
                                                }
                                                else
                                                {
                                                    if (g.DpiX > 120)
                                                    {
                                                        if (item is ToolStripButton && (item as ToolStripButton).DisplayStyle == ToolStripItemDisplayStyle.Image)
                                                        {
                                                            scalSize = new Size(28, 28);
                                                        }
                                                        else if (item is ToolStripSplitButtonEx)
                                                        {
                                                            scalSize = new Size(40, 28);
                                                        }
                                                        else if (item is ToolStripSeparator)
                                                        {
                                                            scalSize = new Size(4, 32);
                                                        }
                                                        item.AutoSize = false;
                                                        item.Size = scalSize;
                                                        szItem = scalSize;
                                                    }
                                                    else if (g.DpiX > 96)
                                                    {
                                                        if (item is ToolStripButton && (item as ToolStripButton).DisplayStyle == ToolStripItemDisplayStyle.Image)
                                                        {
                                                            scalSize = new Size(26, 26);
                                                        }
                                                        else if (item is ToolStripSplitButtonEx)
                                                        {
                                                            scalSize = new Size(35, 24);
                                                        }
                                                        else if (item is ToolStripSeparator)
                                                        {
                                                            scalSize = new Size(3, 26);
                                                        }
                                                        item.AutoSize = false;
                                                        item.Size = scalSize;
                                                        szItem = scalSize;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    int height = szItem.Height + item.Margin.Vertical;
                                    int width = szItem.Width + item.Margin.Horizontal;

                                    if (g.DpiX > 120)
                                    {
                                        height = szItem.Height + item.Margin.Vertical + DPI_150_Gap;//Panel Item Gap
                                        width = szItem.Width + item.Margin.Horizontal + DPI_150_Gap;
                                    }
                                    else if (g.DpiX > 96)
                                    {
                                        height = szItem.Height + item.Margin.Vertical + DPI_125_Gap;
                                        width = szItem.Width + item.Margin.Horizontal + DPI_125_Gap;//Panel Item Gap
                                    }

								if( m_aRows[row] < height )
								{
									m_aRows[row] = height;
								}
								if( m_aCols[col] < width )
								{
									m_aCols[col] = width;
								}

								if( ++row >= nRows )
								{
									col++;
									row = 0;
								}
                                }
							}
						}
					}
				}
				#endregion

				#region Properties
				/// <summary>
				/// 
				/// </summary>
				public Size Size
				{
					get
					{
						return new Size( this.Width, this.Height );
					}
				}
				/// <summary>
				/// 
				/// </summary>
				public int Width
				{
					get
					{
						if( m_Width < 0 )
						{
							m_Width = 0;
							for( int i = 0; i < m_nCols; i++ )
							{
								m_Width += m_aCols[i];
							}
						}
						return m_Width;
					}
				}
				/// <summary>
				/// 
				/// </summary>
				public int Height
				{
					get
					{
						if( m_Height < 0 )
						{
							m_Height = 0;
							for( int i = 0; i < m_nRows; i++ )
							{
								m_Height += m_aRows[i];
							}
						}
						return m_Height;
					}
				}
				/// <summary>
				/// 
				/// </summary>
				public int Rows
				{
					get
					{
						return m_nRows;
					}
				}
				/// <summary>
				/// 
				/// </summary>
				public int Columns
				{
					get
					{
						return m_nCols;
					}
				}
				/// <summary>
				/// 
				/// </summary>
				public int[] RowsData
				{
					get
					{
						return m_aRows;
					}
				}
				/// <summary>
				/// 
				/// </summary>
				public int[] ColumnsData
				{
					get
					{
						return m_aCols;
					}
				}
				#endregion

				#region Fields
				/// <summary>
				/// Rows count
				/// </summary>
				int m_nRows = 0;
				/// <summary>
				/// Columns count
				/// </summary>
				int m_nCols = 0;
				/// <summary>
				/// Rows height
				/// </summary>
				int[] m_aRows = null;
				/// <summary>
				/// Columns width
				/// </summary>
				int[] m_aCols = null;
				/// <summary>
				/// Total width
				/// </summary>
				int m_Width = -1;
				/// <summary>
				/// Total height
				/// </summary>
				int m_Height = -1;
				#endregion
			}
			#endregion

			#region *** PanelItemLayout
			class PanelItemLayout: LayoutEngine
			{
				#region Initialization
				/// <summary>
				/// 
				/// </summary>
				public PanelItemLayout()
				{
				}
				#endregion

				#region Overrides
				/// <summary>
				/// 
				/// </summary>
				/// <param name="child"></param>
				/// <param name="specified"></param>
				public override void InitLayout( object child, BoundsSpecified specified )
				{
				}
				/// <summary>
				/// 
				/// </summary>
				/// <param name="container"></param>
				/// <param name="layoutEventArgs"></param>
				/// <returns></returns>
				public override bool Layout( object container, LayoutEventArgs layoutEventArgs )
				{
					bool bResult = false;

					ToolStripInternal ts = container as ToolStripInternal;
					RightToLeft rtl = ts.RightToLeft;

					if( ts != null )
					{
						LayoutData lData = new LayoutData( ts );

						ToolStripItemCollection items = ts.Items;
						int count = items.Count;

						if( count > 0 )
						{
							int nRows = lData.Rows;
							int nCols = lData.Columns;

							int[] aRows = lData.RowsData;
							int[] aCols = lData.ColumnsData;

							Point pt = new Point( 0, ts.Padding.Top );

							for( int i = 0, row = 0, col = 0; i < count; i++ )
							{
								ToolStripItem item = items[i];

								if( item != null && item.Available )
								{
									if( item.AutoSize )
									{
										item.Size = item.GetPreferredSize( Size.Empty );
									}

									SetItemLocationRTL( ts, item, rtl, pt );

									if( row < nRows - 1 )
									{
										pt.Y += aRows[row++];
									}
									else
									{
										pt.X += aCols[col++];
										pt.Y = ts.Padding.Top;

										row = 0;
									}
								}
							}
						}

						ts.m_layoutData = lData;

						bResult = ts.AutoSize;
					}

					return bResult;
				}
				#endregion

				#region Implementation
				/// <summary> Set item location depending on RTL property. </summary>
				/// <param name="ts"> Toolstrip on which items lay out.</param>
				/// <param name="item"> Item which must be positioned. </param>
				/// <param name="rtl"> ToolStrip RightToLeft property. </param>
				/// <param name="location"> Position for item on ToolStrip. </param>
				private void SetItemLocationRTL( ToolStripInternal ts, ToolStripItem item, RightToLeft rtl, Point location )
				{
					if( rtl == RightToLeft.Yes )
					{
						location.X = ts.DisplayRectangle.Right - location.X - item.Width - item.Margin.Right;
					}
					else
					{
						location.X = ts.DisplayRectangle.X + location.X + item.Margin.Left;
					}

					location.Y = location.Y + item.Margin.Top;

					ts.SetItemLocation( item, location );
				}
				#endregion

				#region Properties
				/// <summary>
				/// 
				/// </summary>
				/// <param name="ts"></param>
				/// <returns></returns>
				public Size GetPrefferedSize( ToolStripInternal ts )
				{
					if( ts.m_layoutData == null )
					{
						ts.m_layoutData = new LayoutData( ts );
					}
					return ts.m_layoutData.Size + ts.Padding.Size;
				}
				#endregion

				#region Fields
				/// <summary> Instance of PanelItemLayout. </summary>
				static public PanelItemLayout Instance = new PanelItemLayout();
				#endregion
			}
			#endregion

			#endregion

			#region Constructors
			/// <summary>
			/// 
			/// </summary>
			/// <param name="bUseStandardLayout"></param>
			public ToolStripInternal( bool bUseStandardLayout )
			{
				m_bUseStandardLayout = bUseStandardLayout;

				this.ShowCaption = false;

				this.Padding = new Padding( DEF_PADDING );
				this.GripStyle = ToolStripGripStyle.Hidden;
				this.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="szProposed"></param>
			/// <returns></returns>
			public override Size GetPreferredSize( Size szProposed )
			{
				PanelItemLayout layout = this.LayoutEngine as PanelItemLayout;
				if( layout != null )
				{
					Size szResult = layout.GetPrefferedSize( this );

					int nBorderWidth = 2 * this.BorderWidth;

					szResult.Width += nBorderWidth;
					szResult.Height += nBorderWidth + this.CaptionHeight;

					return szResult;
				}
				return base.GetPreferredSize( szProposed );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnHandleCreated( EventArgs e )
			{
				base.OnHandleCreated( e );
				PerformLayout();
			}
			/// <summary>
			/// Performance improvement
			/// </summary>
			/// <param name="e"></param>
			protected override void OnVisibleChanged( EventArgs e )
			{
				SuspendLayout();

				base.OnVisibleChanged( e );

				ResumeLayout( m_bUseStandardLayout );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnItemClicked( ToolStripItemClickedEventArgs e )
			{
				base.OnItemClicked( e );

				ToolStripDropDown dropDown = GetParent() as ToolStripDropDown;
				if( dropDown != null && dropDown.AutoClose )
				{
					ToolStripDropDownItem item = e.ClickedItem as ToolStripDropDownItem;
					if( item == null || !item.HasDropDownItems || ( item is ToolStripSplitButton && !item.DropDown.Visible ) )
					{
						dropDown.Close( ToolStripDropDownCloseReason.ItemClicked );
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnItemAdded( ToolStripItemEventArgs e )
			{
				base.OnItemAdded( e );

				ToolStripDropDownItem item = e.Item as ToolStripDropDownItem;
				if( item != null && item.Site == null )
				{
					item.DropDownOpening += new EventHandler( OnItemDropDownOpening );
					item.DropDownClosed += new EventHandler( OnItemDropDownClosed );
				}
				ToolStripGallery gallery = e.Item as ToolStripGallery;
				if( gallery != null )
				{
					gallery.DropDownOpening += new CancelEventHandler( OnGalleryDropDownOpening );
					gallery.DropDownClosed += new ToolStripDropDownClosedEventHandler( OnGalleryDropDownClosed );
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnItemRemoved( ToolStripItemEventArgs e )
			{
				ToolStripDropDownItem item = e.Item as ToolStripDropDownItem;
				if( item != null && item.Site == null )
				{
					item.DropDownOpening -= new EventHandler( OnItemDropDownOpening );
					item.DropDownClosed -= new EventHandler( OnItemDropDownClosed );
				}
				ToolStripGallery gallery = e.Item as ToolStripGallery;
				if( gallery != null )
				{
					gallery.DropDownOpening -= new CancelEventHandler( OnGalleryDropDownOpening );
					gallery.DropDownClosed -= new ToolStripDropDownClosedEventHandler( OnGalleryDropDownClosed );
				}
				base.OnItemRemoved( e );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnPaintBackground( PaintEventArgs e )
			{
				ToolStrip ts = GetBackgroundParent();
                if (ts != null && !SystemInformation.HighContrast )
				{
					ts.Renderer.DrawToolStripBackground( new PanelItemRenderEventArgs( e.Graphics, ts, this ) );
				}
				else
					base.OnPaintBackground( e );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			protected override void WndProc( ref Message m )
			{
				switch( (Msg)m.Msg )
				{
					case Msg.WM_MOUSEACTIVATE:
					m.Result = new IntPtr( (int)MouseActivateFlags.MA_NOACTIVATE );
					return;
					case Msg.WM_MOUSEMOVE:
					if( this.Parent != null )
					{
						Win32API.WindowsAPI.PostMessage( this.Parent.Handle, (int)UtilMsg.WM_HOVERITEM, 0, 0 );
					}
					break;
					case Msg.WM_MOUSELEAVE:
					if( this.Parent != null )
					{
						Win32API.WindowsAPI.PostMessage( this.Parent.Handle, (int)UtilMsg.WM_LEAVEITEM, 0, 0 );
					}
					break;
				}
				base.WndProc( ref m );
			}
            protected override void OnMouseEnter(EventArgs e)
            {

                if (this.Parent is MiniToolBar)
                {

                    this.Focus();               

                }
               
                    base.OnMouseEnter(e);
                
            }
            
            protected override void OnMouseMove(MouseEventArgs e)
            {
                foreach (ToolStripItem t in this.Items)
                {
                    if (t!= null && t is ToolStripPanelItem)
                    {
                        ToolStripPanelItem p = t as ToolStripPanelItem;
                        ToolStrip s1 = p.Parent as ToolStrip;

                        if (s1!=null && s1.TopLevelControl is MiniToolBar)
                        {
                            if (p!=null && p.ShowItemToolTips)
                                s1.ShowItemToolTips = false;
                        }
                    }
                }
               
                base.OnMouseMove(e);
            }
			#endregion

			#region IMessageFilter implementation
			public bool ProcessMessage( ref Message m )
			{
				bool bResult = false;

				switch( (Msg)m.Msg )
				{
					case Msg.WM_WINDOWPOSCHANGING:
					if (this.Handle == m.HWnd)
					{
					WINDOWPOS winPos = (WINDOWPOS)Marshal.PtrToStructure(m.LParam, typeof(WINDOWPOS));
					winPos.flags |= SetWindowPosFlags.SWP_NOMOVE;
					Marshal.StructureToPtr( winPos, m.LParam, false );

					m.Result = IntPtr.Zero;
					bResult = true;
					}
					break;
				}

				return bResult;
			}
			#endregion

			#region Implementation
			ToolStrip GetParent()
			{
				ToolStrip ts = this.Parent as ToolStrip;

				while( ts is ToolStripInternal )
				{
					ts = ts.Parent as ToolStrip;
				}

				return ts;
			}
			ToolStrip GetBackgroundParent()
			{
				ToolStrip ts = this;

				while( ts is ToolStripInternal && ( (ToolStripInternal)ts ).Transparent )
				{
					ts = ts.Parent as ToolStrip;
				}

				return ts;
			}
			#endregion

			#region Event Handlers
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnItemDropDownOpening( object sender, EventArgs e )
			{
				ToolStripDropDownItem item = sender as ToolStripDropDownItem;

				if( item != null )
				{
					item.DropDown.Opening += new CancelEventHandler( OnDropDownOpening );
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnDropDownOpening( object sender, CancelEventArgs e )
			{
				ToolStrip parent = GetParent();
				if( parent is ToolStripDropDown )
				{
					ToolStripDropDown ts = sender as ToolStripDropDown;
					if( ts != null && ts.OwnerItem != null )
					{
						SetItemParent( ts.OwnerItem, parent );
						Invalidate( ts.OwnerItem.Bounds );

						this.DropDownWindow.MessageFilter = this;

						if( !this.DropDownWindow.IsAssigned )
						{
							this.DropDownWindow.Assign( ts.Handle );
						}

						ts.Opening -= new CancelEventHandler( OnDropDownOpening );
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnItemDropDownClosed( object sender, EventArgs e )
			{
				if( this.DropDownWindow.MessageFilter != null )
				{
					ToolStripDropDownItem dropDownItem = sender as ToolStripDropDownItem;

					if( dropDownItem != null )
					{
						ToolStripDropDown ts = dropDownItem.DropDown;
						if( ts != null )
						{
							ToolStripItem item = ts.OwnerItem;
							if( item != null )
							{
								SetItemParent( item, this );
								item.Invalidate();
							}
						}
					}
					this.DropDownWindow.MessageFilter = null;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnGalleryDropDownOpening( object sender, CancelEventArgs e )
			{
				ToolStrip parent = GetParent();
				if( parent is ToolStripDropDown )
				{
					ToolStripDropDown ts = sender as ToolStripDropDown;
					if( ts != null && ts.OwnerItem != null )
					{
						SetItemParent( ts.OwnerItem, parent );
						Invalidate( ts.OwnerItem.Bounds );
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnGalleryDropDownClosed( object sender, ToolStripDropDownClosedEventArgs e )
			{
				ToolStripDropDown ts = sender as ToolStripDropDown;
				if( ts != null )
				{
					ToolStripItem item = ts.OwnerItem;
					if( item != null )
					{
						SetItemParent( item, this );
						item.Invalidate();
					}
				}
			}
			#endregion

			#region Properties
			public override LayoutEngine LayoutEngine
			{
				get
				{
					if( m_bUseStandardLayout )
					{
						return base.LayoutEngine;
					}
					return PanelItemLayout.Instance;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			protected override CreateParams CreateParams
			{
				[SecurityPermission( SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode )]
				get
				{
					CreateParams result = base.CreateParams;
					return result;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			private NativeMessageHandler DropDownWindow
			{
				get
				{
					if( m_dropDown == null )
					{
						m_dropDown = new NativeMessageHandler();
					}
					return m_dropDown;
				}
			}
			/// <summary>
			/// Gets or sets value indicating whether control is transparent.
			/// </summary>
			public bool Transparent
			{
				get
				{
					return m_bTransparent;
				}
				set
				{
					m_bTransparent = value;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public bool UseStandardLayout
			{
				get
				{
					return m_bUseStandardLayout;
				}
				set
				{
					if( m_bUseStandardLayout != value )
					{
						m_bUseStandardLayout = value;
						PerformLayout();
					}
				}
			}
            private bool defaultDPISize = false;
            /// <summary>
            /// Gets or sets value for Default size.
            /// </summary>
            [Category("Layout"), Description("Specifies the size of the items")]
            [DefaultValue(false)]
            internal bool DefaultDPISize
            {
                get
                {
                    return defaultDPISize;
                }
                set
                {
                    defaultDPISize = value;
                    PerformLayout();
                }
            }
			/// <summary>
			/// 
			/// </summary>
			public int RowCount
			{
				get
				{
					return m_rowCount;
				}
				set
				{
					if( m_rowCount != value )
					{
						m_rowCount = value;

						PerformLayout( this, "RowCount" );
					}
				}
			}
			#endregion

			#region Fields
			NativeMessageHandler m_dropDown = null;
			private LayoutData m_layoutData = null;
			private int m_rowCount = DEF_ROWCOUNT;
			private bool m_bTransparent = true;
			private bool m_bUseStandardLayout = false;
			#endregion
		}
		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		public ToolStripPanelItem()
			: this( false )
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bUseStandardLayout"></param>
		public ToolStripPanelItem( bool bUseStandardLayout )
			: base( new ToolStripInternal( bUseStandardLayout ) )
		{
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="proposedSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize( Size proposedSize )
		{
			if( this.Items.Count > 0 )
			{
				return this.ToolStrip.GetPreferredSize( proposedSize );
			}
			return new Size( 0x17, 0x17 );
		}
		/// <summary>
		/// 
		/// </summary>
		protected override void OnBoundsChanged()
		{
			this.ToolStrip.SetBounds( this.Bounds.X, this.Bounds.Y, this.Width, this.Height );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnOwnerChanged( EventArgs e )
		{
			base.OnOwnerChanged( e );
			SetOwner( this.Owner );
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnOwnerRendererChanged( object sender, EventArgs e )
		{
			if( !this.IsDisposed )
			{
				ToolStrip ts = sender as ToolStrip;

				if( ts != null )
				{
					if( ts.RenderMode == ToolStripRenderMode.ManagerRenderMode )
					{
						this.ToolStrip.Renderer = null;
					}
					else
					{
						this.ToolStrip.Renderer = ts.Renderer;
					}
				}
			}
		}
		#endregion

		#region Implementation
		void SetOwner( ToolStrip newOwner )
		{
			if( m_tsOwner != null )
			{
				m_tsOwner.RendererChanged -= new EventHandler( OnOwnerRendererChanged );
			}
			if( newOwner != null )
			{
				newOwner.RendererChanged += new EventHandler( OnOwnerRendererChanged );
				OnOwnerRendererChanged( newOwner, EventArgs.Empty );
			}
			m_tsOwner = newOwner;
		}
		#endregion

		#region Events
		/// <summary>
		/// Fires when item in this controls is clicked.
		/// </summary>
		public event ToolStripItemClickedEventHandler ItemClicked
		{
			add
			{
				(this.Control as ToolStrip).ItemClicked += value;
			}
			remove
			{
				(this.Control as ToolStrip).ItemClicked -= value;
			}
		}
		#endregion
		#region Properties
		/// <summary>
		/// Gets the collection of items to display on the ToolStripPanelItem
		/// </summary>
		[Category( "Data" ), Description( "Collection of items to display on the ToolStripPanelItem" )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content ), MergableProperty( false )]
		[Editor( typeof( Design.ToolStripItemsEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
		public ToolStripItemCollection Items
		{
			get
			{
				return ( this.Control as ToolStrip ).Items;
			}
		}
		/// <summary>
		/// 
		/// Gets or sets the maximum number of rows.
		/// </summary>
		[Category( "Layout" ), Description( "Specifies the maximum number of rows" )]
		[DefaultValue( ToolStripPanelItem.DEF_ROWCOUNT )]
		public int RowCount
		{
			get
			{
				return InternalToolStrip.RowCount;
			}
			set
			{
				InternalToolStrip.RowCount = value;
			}
		}
        /// <summary>
        /// Gets or sets value for Default size.
        /// </summary>
        [Category("Layout"), Description("Specifies the size of the items")]
        [DefaultValue(false)]
        public bool DefaultDPISize
        {
            get
            {
                return InternalToolStrip.DefaultDPISize;
            }
            set
            {
                InternalToolStrip.DefaultDPISize = value;
            }
        }
		/// <summary>
		/// Gets/sets whether the buttons should be grouped.
		/// </summary>
		[Category( "Layout" ), DefaultValue( false )]
		[Description( "Gets/sets whether the buttons should be grouped." )]
		public bool GroupedButtons
		{
			get
			{
				return ToolStrip.GroupedButtons;
			}
			set
			{
				ToolStrip.GroupedButtons = value;
			}
		}
		/// <summary>
		/// Gets or sets whether to use Standard Layout.
		/// </summary>
		[Category( "Layout" ), DefaultValue( false )]
		[Description( "Gets or sets whether to use Standard Layout." )]
		public bool UseStandardLayout
		{
			get
			{
				return this.InternalToolStrip.UseStandardLayout;
			}
			set
			{
				this.InternalToolStrip.UseStandardLayout = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Category( "Layout" ), DefaultValue( typeof( ToolStripLayoutStyle ), "HorizontalStackWithOverflow" )]
		public ToolStripLayoutStyle LayoutStyle
		{
			get
			{
				return this.ToolStrip.LayoutStyle;
			}
			set
			{
				this.ToolStrip.LayoutStyle = value;
			}
		}

		/// <summary>
		/// Specifies whether to display ToolTips on items.
		/// </summary>
		[Category( "Behavior" ), DefaultValue( false )]
		[Description( "Specifies whether to display ToolTips on items." )]
		public bool ShowItemToolTips
		{
			get
			{
				return this.ToolStrip.ShowItemToolTips;
			}
			set
			{
				this.ToolStrip.ShowItemToolTips = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Category( "Office mode" )]
		[DefaultValue( typeof( ToolStripBorderStyle ), "None" )]
		public ToolStripBorderStyle BorderStyle
		{
			get
			{
				return this.ToolStrip.BorderStyle;
			}
			set
			{
				this.ToolStrip.BorderStyle = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override Padding Padding
		{
			get
			{
				return this.ToolStrip.Padding;
			}
			set
			{
				this.ToolStrip.Padding = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected override Padding DefaultMargin
		{
			get
			{
				return new Padding( 1, 0, 1, 0 );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[Browsable( false )]
		public ToolStripEx ToolStrip
		{
			get
			{
				return this.Control as ToolStripEx;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private ToolStripInternal InternalToolStrip
		{
			get
			{
				return this.Control as ToolStripInternal;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether control is transparent.
		/// </summary>
		public bool Transparent
		{
			get
			{
				return InternalToolStrip.Transparent;
			}
			set
			{
				InternalToolStrip.Transparent = value;
			}
		}
		#endregion

		#region Shouldserialize/Reset methods
        bool ShouldSerializeDefaultDpiSize()
        {
            return this.DefaultDPISize != false;
        }
		bool ShouldSerializePadding()
		{
			return this.Padding != new Padding( DEF_PADDING );
		}
        void ResetDefaultDpiSize()
        {
            this.DefaultDPISize = false;
        }
		new void ResetPadding()
		{
			this.Padding = new Padding( DEF_PADDING );
		}
		#endregion

		#region Fields
		ToolStrip m_tsOwner = null;
		#endregion
	}
	#endregion
}
#endif