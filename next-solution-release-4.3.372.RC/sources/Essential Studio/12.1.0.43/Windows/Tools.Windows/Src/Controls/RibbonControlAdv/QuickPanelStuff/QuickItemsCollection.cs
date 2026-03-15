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
using System.Collections.Generic;
using System.Text;

using Syncfusion.Windows.Forms.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
	partial class RibbonControlAdvHeader
	{
		internal class QuickItemsCollection : RibbonItemsCollection
		{
			#region Fields
			/// <summary>
			/// 
			/// </summary>
			private ToolStrip m_owner;
			/// <summary>
			/// 
			/// </summary>
			private RibbonControlAdvHeader m_header;
			/// <summary>
			/// Indicates whether toolstrip items must be destroyed on removing from their base collection.
			/// </summary>
			public bool DestroyItemsOnRemove = true;
            /// <summary>
            /// DPI 150 Extra item size
            /// </summary>
            private const int DPI_150_ExtraItemSize = 7;
            /// <summary>
            /// TouchModeWidth 
            /// </summary>
            private const int TouchModeWidth = 24;
            /// <summary>
            /// DPI 125 Extra item size
            /// </summary>
            private const int DPI_125_ExtraItemSize = 4;
			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			public ToolStrip Owner
			{
				get
				{
					return m_owner;
				}
				set
				{
					if( m_owner != value )
					{
						m_owner = value;
						this.DestroyItemsOnRemove = false;
						for( int i = 0, count = this.Count; i < count; i++ )
						{
							this[ i ].Owner = m_owner;
						}
						this.DestroyItemsOnRemove = true;
					}
				}
			}
			#endregion

			#region Initialization
			
			public QuickItemsCollection( RibbonControlAdvHeader header )
			{
				m_header = header;
			}
			#endregion

			#region Public Methods
			/// <summary>
			/// Lays out items in the specified bounds rectangle.
			/// </summary>
			/// <param name="bounds">Rectangle to lay out items in.</param>
			/// <returns>Resulting bounds of layout.</returns>
			public Rectangle Layout( Rectangle bounds )
			{
				Rectangle result = Rectangle.Empty;

				if( m_owner != null )
				{
					result = (m_owner.RightToLeft == RightToLeft.No) ? (LayoutLTR(bounds)) : (LayoutRTL(bounds));
				}

				return result;
			}
			#endregion

			#region Implementation
			/// <summary>
			/// Performs full layout of items, with possible changes in overflows.
			/// </summary>
			/// <param name="bounds">Rectangle to lay out items in.</param>
			/// <returns>Resulting bounds of layout.</returns>
            private Rectangle LayoutLTR(Rectangle bounds)
            {
                Rectangle result = bounds;

                RibbonControlAdvHeader.QuickItemsDropDownButton qaButton = m_header.QuickAccessButton;
                RibbonControlAdvHeader.QuickItemsOverflowButton qaOverflowButton = m_header.QuickOverflowButton;

                // Clear overflow items.
                m_header.OverflowsItems.Clear();

                bool bEnough = false;
                int quickPos = 0;
                using (Graphics g = Graphics.FromImage(new Bitmap(10, 10)))
                {
                    if (m_header.RibbonStyle == RibbonStyle.Office2007)
                    {
                        quickPos = bounds.Left;
                    }
                    else
                    {
                        if (m_header.ribbonTouchModeEnabled)
                        {
                            quickPos = bounds.Left + QUICK_ITEM_SEPARATOR_WIDTH + 3 + TouchModeWidth;
                        }
                        else
                        {
                            if (g.DpiX > 120)
                            {
                                quickPos = bounds.Left + QUICK_ITEM_SEPARATOR_WIDTH + 3 + DPI_150_ExtraItemSize;
                            }
                            else if (g.DpiX > 96)
                            {
                                quickPos = bounds.Left + QUICK_ITEM_SEPARATOR_WIDTH + 3 + DPI_125_ExtraItemSize;
                            }
                            else
                            {
                                quickPos = bounds.Left + QUICK_ITEM_SEPARATOR_WIDTH + 3;
                            }
                        }
                    }

                    if (m_header.QuickPanelVisible)
                    {
                        int itemsize = 0;
                        if (!m_header.ShowQuickPanelBelowRibbon)
                        {
                            m_header.m_quickDropDownButton.ToolTipText = "";
                            m_header.m_quickOverflowButton.ToolTipText = "";
                        }
                        else
                        {
                            m_header.m_quickDropDownButton.ToolTipText = m_header.QuickDropDownToolTipText;
                            m_header.m_quickOverflowButton.ToolTipText = m_header.OverFlowButtonToolTip;
                        }
                        for (int i = 0, count = this.Count; i < count; i++)
                        {
                            ToolStripItem item = this[i];

                            if (item != null && item.Available)
                            {
                                if (item.AutoSize)
                                {
                                    int width = item.GetPreferredSize(bounds.Size).Width;

                                    if (g.DpiX > 120 || m_header.ribbonTouchModeEnabled)
                                    {
                                        width = item.GetPreferredSize(bounds.Size).Width + TouchModeWidth;
                                    }

                                    else if (g.DpiX > 96)
                                    {
                                        width = item.GetPreferredSize(bounds.Size).Width + DPI_125_ExtraItemSize;
                                    }
                                
                                    item.Size = new Size(width, bounds.Height - item.Margin.Vertical);
                                }
                                int len = bounds.Right - item.Width - item.Margin.Horizontal - qaButton.Margin.Horizontal - qaButton.Width + DPI_125_ExtraItemSize;
                                if (m_header.TabGroupsHash.Count > 0)
                                {
                                    foreach (ToolStripTabItem tItem in m_header.TabGroupsHash.Keys)
                                    {
                                        len = tItem.Bounds.X + tItem.Bounds.Width;
                                    }
                                }
                                itemsize = itemsize + quickPos;
                                if (!m_header.ShowQuickPanelBelowRibbon && this.Count > 0)
                                {
                                    if (itemsize >= len || bEnough)
                                    {
                                        if (qaOverflowButton != null)
                                            RibbonControlAdvHeader.SetItemParent(item, qaOverflowButton.DropDown);
                                        m_header.OverflowsItems.Add(item);
                                        bEnough = true;
                                    }
                                    else
                                    {
     
                                        RibbonControlAdvHeader.SetItemParent(item, m_owner);

                                        ((ILayoutSupport)m_owner).SetItemLocation(item, new Point(quickPos, bounds.Top + item.Margin.Top));
                                        quickPos += item.Bounds.Width + item.Margin.Horizontal;
                                    }
                                }
                                else
                                {
                                    if (quickPos > m_header.m_form.Width - 45)
                                    {
                                        if (qaOverflowButton != null)
                                            RibbonControlAdvHeader.SetItemParent(item, qaOverflowButton.DropDown);
                                        m_header.OverflowsItems.Add(item);
                                        bEnough = true;

                                    }
                                    else
                                    {
                                        RibbonControlAdvHeader.SetItemParent(item, m_owner);

                                        ((ILayoutSupport)m_owner).SetItemLocation(item, new Point(quickPos, bounds.Top + item.Margin.Top));

                                        quickPos += item.Bounds.Width + item.Margin.Horizontal;
                                    }
                                }
                            }
                        }
                    }

                    if (!m_header.ShowQuickPanelBelowRibbon && this.Count > 0)
                    {
                        quickPos += bounds.Height / 2;
                    }

                    //Set location of Quick access button.
                    if (qaButton != null)
                    {
                        Padding margin = qaButton.Margin;

                        qaButton.Size = new Size(qaButton.GetPreferredSize(bounds.Size).Width, bounds.Height - margin.Vertical);
                        if (qaOverflowButton != null)
                        {

                            if (m_header.HasOverflowItems)
                            {
                                RibbonControlAdvHeader.SetItemParent(qaButton, qaOverflowButton.DropDown);
                                RibbonControlAdvHeader.SetItemParent(qaOverflowButton, m_owner);
                                m_header.m_quickDropDownButton.ToolTipText = m_header.QuickDropDownToolTipText;
                                m_header.OverflowsItems.Add(qaButton);
                            }
                            else
                            {
                                RibbonControlAdvHeader.SetItemParent(qaButton, m_owner);
                                RibbonControlAdvHeader.SetItemParent(qaOverflowButton, null);
                                qaButton.Location = new Point(quickPos + margin.Left, bounds.Top + margin.Top);
                                result.Width = qaButton.Bounds.Right - bounds.Left;
                            }
                        }
                    }

                    if (qaOverflowButton != null)
                    {
                        Padding overflowMargin = qaOverflowButton.Margin;
                        qaOverflowButton.Size = new Size(qaOverflowButton.GetPreferredSize(bounds.Size).Width,
                            bounds.Height - overflowMargin.Vertical);
                        qaOverflowButton.Location =
                            new Point(quickPos + overflowMargin.Left, bounds.Top + overflowMargin.Top);
                        result.Width = qaOverflowButton.Bounds.Right - bounds.Left;
                    }
                }

                return result;
            }
			/// <summary>
			/// 
			/// </summary>
			/// <param name="bounds"></param>
			/// <returns></returns>
			private Rectangle LayoutRTL( Rectangle bounds )
			{
				Rectangle result = bounds;
                const int itemWidth = 25;
				RibbonControlAdvHeader.QuickItemsDropDownButton qaButton = m_header.QuickAccessButton;
				RibbonControlAdvHeader.QuickItemsOverflowButton qaOverflowButton = m_header.QuickOverflowButton;

				// Clear overflow items.
				m_header.OverflowsItems.Clear();

				bool bEnough = false;

				int quickPos = 0;
				if (m_header.RibbonStyle ==RibbonStyle .Office2007)
				{
					quickPos = bounds.Right - 1 * (QUICK_ITEM_SEPARATOR_WIDTH - 3);
				}
				else
				{
					quickPos = bounds.Right + 4 * (QUICK_ITEM_SEPARATOR_WIDTH + 3);
				}

				if( m_header.QuickPanelVisible )
				{
					for( int i = 0, count = this.Count; i < count; i++ )
					{
						ToolStripItem item = this[ i ];

						if (item != null && item.Available)
						{
							if( item.AutoSize )
							{
								int width = item.GetPreferredSize( bounds.Size ).Width;
                                using (Graphics g = Graphics.FromImage(new Bitmap(10, 10)))
                                {
                                    if (m_header.ribbonTouchModeEnabled)
                                    {
                                        width = item.GetPreferredSize(bounds.Size).Width + TouchModeWidth;
                                    }
                                    else
                                    {
                                        if (g.DpiX > 120 || m_header.ribbonTouchModeEnabled)
                                        {
                                            width = item.GetPreferredSize(bounds.Size).Width + DPI_150_ExtraItemSize;
                                        }

                                        else if (g.DpiX > 96)
                                        {
                                            width = item.GetPreferredSize(bounds.Size).Width + DPI_125_ExtraItemSize;
                                        }
                                    }
                                }
								item.Size = new Size( width, bounds.Height - item.Margin.Vertical );
							}

							int len = item.Width + item.Margin.Horizontal + qaButton.Margin.Horizontal + qaButton.Width;
                            if (!m_header.ShowQuickPanelBelowRibbon && this.Count > 0)
                            {
                                if (quickPos <= len + bounds.Left || bEnough)
                                {
                                    RibbonControlAdvHeader.SetItemParent(item, qaOverflowButton.DropDown);
                                    m_header.OverflowsItems.Add(item);
                                    bEnough = true;
                                }
                                else
                                {
                                    RibbonControlAdvHeader.SetItemParent(item, m_owner);
                                    m_header.m_quickDropDownButton.ToolTipText = "";
                                    m_header.m_quickOverflowButton.ToolTipText = "";
                                    if (m_header.ShowQuickPanelBelowRibbon)
                                        ((ILayoutSupport)m_owner).SetItemLocation(
                                            item, new Point(quickPos - 2 * item.Width - item.Margin.Right, bounds.Top + item.Margin.Top));
                                    else
                                        ((ILayoutSupport)m_owner).SetItemLocation(
                                        item, new Point(quickPos - item.Width - item.Margin.Right, bounds.Top + item.Margin.Top));
                                    quickPos -= item.Bounds.Width + item.Margin.Horizontal;
                                }
                            }
                            else
                            {
                                if (quickPos <= m_header.m_form.Width - 45)
                                {
                                    RibbonControlAdvHeader.SetItemParent(item, qaOverflowButton.DropDown);
                                    m_header.OverflowsItems.Add(item);
                                    bEnough = true;
                                }
                                else
                                {
                                    RibbonControlAdvHeader.SetItemParent(item, m_owner);
                                    m_header.m_quickDropDownButton.ToolTipText = m_header.QuickDropDownToolTipText;
                                    m_header.m_quickOverflowButton.ToolTipText = m_header.OverFlowButtonToolTip;
                                    ((ILayoutSupport)m_owner).SetItemLocation(
                                        item, new Point(quickPos - item.Width - item.Margin.Right, bounds.Top + item.Margin.Top));
                                    quickPos -= item.Bounds.Width + item.Margin.Horizontal;
                                }
                            }
						}
					}

					if( !m_header.ShowQuickPanelBelowRibbon && this.Count > 0 )
					{
						quickPos -= bounds.Height / 2;
					}

					//Set location of Quick access button.
					if( qaButton != null )
					{
						Padding margin = qaButton.Margin;

						qaButton.Size = new Size( qaButton.GetPreferredSize( bounds.Size ).Width, bounds.Height - margin.Vertical );

						if( m_header.HasOverflowItems )
						{
							RibbonControlAdvHeader.SetItemParent( qaButton, qaOverflowButton.DropDown );
							RibbonControlAdvHeader.SetItemParent( qaOverflowButton, m_owner );
							m_header.OverflowsItems.Add( qaButton );
						}
						else
						{
							RibbonControlAdvHeader.SetItemParent( qaButton, m_owner );
							RibbonControlAdvHeader.SetItemParent( qaOverflowButton, null );
                            if (m_header.ShowQuickPanelBelowRibbon && m_header.RibbonStyle != RibbonStyle.Office2007)
                                qaButton.Location = new Point(quickPos - qaButton.Width - margin.Right - itemWidth, bounds.Top + margin.Top);
                            else
                                qaButton.Location = new Point(quickPos - qaButton.Width - margin.Right, bounds.Top + margin.Top);
							result.Width = bounds.Right - qaButton.Bounds.Left;
							result.X = qaButton.Bounds.Left;
						}
					}

					if( qaOverflowButton != null )
					{
						Padding overflowMargin = qaOverflowButton.Margin;
						qaOverflowButton.Size = new Size( qaOverflowButton.GetPreferredSize( bounds.Size ).Width,
							bounds.Height - overflowMargin.Vertical );
						qaOverflowButton.Location = new Point( quickPos - qaOverflowButton.Width - overflowMargin.Right,
							bounds.Top + overflowMargin.Top );
						result.Width = bounds.Right - qaOverflowButton.Bounds.Left;
						result.X = qaOverflowButton.Bounds.Left;
					}
				}

				return result;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="idx"></param>
			/// <returns></returns>
			private string GetQatAccelerator(int idx)
			{
				if (idx >= 10)
				{
					StringBuilder str = new StringBuilder(255);
					
					int a = Convert.ToInt32('A');

					for (int i = idx; i > 0; )
					{
						int iByte = i % 36;

						if (iByte >= 10)
						{
							str.Insert(0, Convert.ToChar(a + (iByte - 10)) );
						}
						else str.Insert(0, iByte.ToString());

						i = i / 36;
					}
					str.Insert(0,"0");

					return str.ToString();
				}

				return idx.ToString();
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="removed"></param>
			internal void UpdateAccelerators(object removed)
			{
				RibbonControlAdvHeader header = this.Owner as RibbonControlAdvHeader;
				if (header != null)
				{
					SuperAccelerator accelerator = header.SuperAccelerator;
					if (accelerator != null)
					{
                        accelerator.SetAccelerator(removed as ToolStripItem, null);

						int idx = 1;

						foreach (object obj in this.List)
						{
							ToolStripItem item = obj as ToolStripItem;
							if (item != null)
							{
								accelerator.SetAccelerator(item, GetQatAccelerator(idx));
								idx++;
							}
						}
					}
				}
			}


			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			protected override bool OnRemove( int index, object value )
			{
				return base.OnRemove( index, value );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			protected override bool OnInsert( int index, object value )
			{
				bool bResult = base.OnInsert(index, value);
				
				if(bResult)
				{
					ToolStripItem item = value as ToolStripItem;
					if( item != null )
					{
						item.Owner = m_owner;
					}
				}
				return bResult;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			protected override void OnInsertComplete(int index, object value)
			{
				base.OnInsertComplete(index, value);

				ToolStripItem item = value as ToolStripItem;

				if (item != null)
				{
					IQuickItem quickItem = item as IQuickItem;

					if (quickItem != null)
					{
						SuperToolTip.SetToolTips(item, quickItem.ReflectedComponent);
					}

					UpdateAccelerators(null);

					if (_ItemAdded != null)
					{
						_ItemAdded(m_header, new ToolStripItemEventArgs(item));
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			protected override void OnRemoveComplete(int index, object value)
			{
				base.OnRemoveComplete(index, value);

				ToolStripItem item = value as ToolStripItem;

				if (item != null)
				{
					if (item is IQuickItem)
					{
						SuperToolTip.RemoveToolTips(item);
					}

					UpdateAccelerators(value);

					if (_ItemRemoved != null)
					{
						_ItemRemoved(m_header, new ToolStripItemEventArgs(item));
					}
				}
			}
			#endregion

			#region Events
			public event ToolStripItemEventHandler _ItemAdded;
			public event ToolStripItemEventHandler _ItemRemoved;
			#endregion
		}
	}
}
#endif
