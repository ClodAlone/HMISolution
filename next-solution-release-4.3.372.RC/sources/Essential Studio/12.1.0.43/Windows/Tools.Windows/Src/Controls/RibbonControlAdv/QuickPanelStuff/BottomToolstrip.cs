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
using System.Windows.Forms;
using System.Drawing;

using Syncfusion.Windows.Forms.Tools.Win32API;
using System.Windows.Forms.Layout;
using Syncfusion.Windows.Forms.Collections;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Toolstrip that contains quick items when they are places below ribbon.
	/// </summary>
	internal class BottomToolstrip
		: ToolStrip
		, ILayoutSupport
	{
		#region Internal Classes
		/// <summary>
		/// 
		/// </summary>
		private class BottomToolstripLayoutEngine
			: LayoutEngine
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="container"></param>
			/// <param name="layoutEventArgs"></param>
			/// <returns></returns>
			public override bool Layout( object container, LayoutEventArgs layoutEventArgs )
			{
				BottomToolstrip toolstrip = container as BottomToolstrip;
				if( toolstrip != null )
				{
					RibbonControlAdv ribbon = toolstrip.Parent as RibbonControlAdv;
					if( ribbon != null && ribbon.ShowQuickPanelBelowRibbon )
					{
						toolstrip.m_quickItems.Owner = toolstrip;
                        using (Graphics g = Graphics.FromImage(new Bitmap(10, 10)))
                        {
                            if (ribbon.HeaderInternal.RibbonTouchModeEnabled)
                            {
                                toolstrip.ClientSize = new Size(toolstrip.ClientSize.Width,
                           RibbonControlAdvHeader.TouchItemSize(toolstrip.m_quickItems).Height + toolstrip.Padding.Vertical);
                            }
                            else
                            {
                                if (g.DpiX > 120 || ribbon.HeaderInternal.RibbonTouchModeEnabled)
                                {
                                    toolstrip.ClientSize = new Size(toolstrip.ClientSize.Width,
                           RibbonControlAdvHeader.GetItems150DPISize(toolstrip.m_quickItems).Height + toolstrip.Padding.Vertical);
                                }
                                else if (g.DpiX > 96)
                                    toolstrip.ClientSize = new Size(toolstrip.ClientSize.Width,
                           RibbonControlAdvHeader.GetItems125DPISize(toolstrip.m_quickItems).Height + toolstrip.Padding.Vertical);
                                else
                                    toolstrip.ClientSize = new Size(toolstrip.ClientSize.Width,
                           RibbonControlAdvHeader.GetItemsSize(toolstrip.m_quickItems).Height + toolstrip.Padding.Vertical);
                            }
                        }
						Rectangle rect = new Rectangle();
						rect.Y = toolstrip.Padding.Top;
						rect.X = toolstrip.Padding.Left;
						rect.Width = toolstrip.ClientSize.Width - toolstrip.Padding.Horizontal;
						rect.Height = toolstrip.ClientSize.Height - toolstrip.Padding.Vertical;

						toolstrip.m_quickItems.Layout( rect );
					}
				}

				return false;
			}
		}
		#endregion
		
		#region Fields
        /// <summary>
        /// Specifis whether QuickItemsDropDownButton need to be shown.
        /// </summary>
        private bool m_ShowQuickItemsDropDownButton = true;
		/// <summary>
		/// Fake item to return for invisible items during filtering.
		/// </summary>
		private ToolStripItem m_fakeHiddenItem;
		/// <summary>
		/// Collection of filtered items.
		/// </summary>
		private RibbonControlAdvHeader.ToolStripItemCollectionFiltered m_filteredItems;
		/// <summary>
		/// Indicates whether items should be filtered.
		/// </summary>
		private bool m_bHideItems;
		/// <summary>
		/// 
		/// </summary>
		private RibbonControlAdvHeader.QuickItemsCollection m_quickItems;
		/// <summary>
		/// 
		/// </summary>
		private BottomToolstripLayoutEngine m_layoutEngine;
		/// <summary>
		/// 
		/// </summary>
		private	bool m_bReadyForLayout = false;
		#endregion

        #region Properties
        /// <summary>
        /// Specifies whether QuickItemsDropDownButton need to be shown.
        /// </summary>
        [DefaultValue(true)]
        internal bool ShowQuickItemsDropDownButton
        {
            get
            {
                return this.m_ShowQuickItemsDropDownButton;
            }
            set
            {
                if (this.m_ShowQuickItemsDropDownButton != value)
                {
                    this.m_ShowQuickItemsDropDownButton = value;
                    this.SetDisplayedItems();
                }
            }
        }
        #endregion

        #region Initialization
        /// <summary>
		/// 
		/// </summary>
		public BottomToolstrip( RibbonControlAdvHeader.QuickItemsCollection quickItems )
		{
			m_quickItems = quickItems;
			m_quickItems.ItemAdded += new EventHandler<ListItemEventArgs<ToolStripItem>>( OnQuickItemsChanged );
			m_quickItems.ItemRemoved += new EventHandler<ListItemEventArgs<ToolStripItem>>( OnQuickItemsChanged );

			m_layoutEngine = new BottomToolstripLayoutEngine();

			m_fakeHiddenItem = new ToolStripButton();
			m_fakeHiddenItem.Available = false;
			this.Items.Add( m_fakeHiddenItem );

			this.Margin = new Padding( 4, 2, 4, 0 );
			this.Padding = new Padding( 2, 2, 2, 2 );
			this.CanOverflow = false;
			m_bReadyForLayout = true;
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// 
		/// </summary>
		internal void UpdateRenderer()
		{
			RibbonControlAdv parent = this.Parent as RibbonControlAdv;
			if( parent != null )
			{
                if (parent.RibbonStyle == RibbonStyle.Office2007 || parent.RibbonStyle == RibbonStyle.Office2010)
                {
                    switch (parent.OfficeColorScheme)
                    {
                        case ToolStripEx.ColorScheme.Silver:
                            this.Renderer = new Office12ToolStripRenderer(new Office12ColorTable());
                            break;
                        case ToolStripEx.ColorScheme.Blue:
                            this.Renderer = new Office12ToolStripRenderer(new OfficeBlue());
                            break;
                        case ToolStripEx.ColorScheme.Black:
                            this.Renderer = new Office12ToolStripRenderer(new OfficeBlack());
                            break;
                        default:
                            this.Renderer = new Office12ToolStripRenderer(Office12ColorTable.ManagedColors);
                            break;
                    }
                }
                else
                {
                    this.Renderer = new Office2013ToolStripRenderer();
                    (this.Renderer as Office2013ToolStripRenderer).MenuColor=ControlPaint.LightLight(parent.MenuColor);
                    (this.Renderer as Office2013ToolStripRenderer).UseDefaultHighlightColor = parent.UseDefaultHighlightColor;
                    (this.Renderer as Office2013ToolStripRenderer).ToolStipOffice2013ColorScheme = parent.Office2013ColorScheme;
                }
			}
		}
		#endregion

		#region Overrides
        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            base.OnItemClicked(e);
        }
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_filteredItems != null)
				{
					m_filteredItems.Dispose();
					m_filteredItems = null;
				}
                this.Renderer = null;
			}
			base.Dispose(disposing);
		}
		/// <summary>
		/// 
		/// </summary>
		protected override void SetDisplayedItems()
		{
			m_bHideItems = true;
			base.SetDisplayedItems();
			m_bHideItems = false;

			RibbonControlAdv parent = this.Parent as RibbonControlAdv;
			if( parent != null )
			{
				if( parent.HeaderInternal.HasOverflowItems )
				{
                    if (parent.HeaderInternal.QuickOverflowButton != null)
					this.DisplayedItems.Add(parent.HeaderInternal.QuickOverflowButton);
				}
				else
				{
                    if(this.ShowQuickItemsDropDownButton)
					    this.DisplayedItems.Add(parent.HeaderInternal.QuickAccessButton);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override System.Drawing.Rectangle DisplayRectangle
		{
			get
			{
				Rectangle result = base.DisplayRectangle;
				result.Width += result.X;
				result.X = 0;
				return result;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override ToolStripItemCollection Items
		{
			get
			{
				ToolStripItemCollection items = base.Items;

				if (m_bHideItems && !this.Disposing)
				{
					if (m_filteredItems == null)
					{
						m_filteredItems = new RibbonControlAdvHeader.ToolStripItemCollectionFiltered( this, items );
					}
					items = m_filteredItems;
				}

				return items;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override System.Windows.Forms.Layout.LayoutEngine LayoutEngine
		{
			get
			{
				if( m_bReadyForLayout )
				{
					return m_layoutEngine;
				}

				return base.LayoutEngine;
			}
		}
		#endregion

		#region IItemLocationSetable Members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <param name="location"></param>
		public new void SetItemLocation( ToolStripItem item, System.Drawing.Point location )
		{
			if( item.Owner == this )
			{
				base.SetItemLocation( item, location );
			}
		}
		#endregion

		#region IItemHidable Members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public ToolStripItem GetItem( ToolStripItem item )
		{
			bool bHide = ( RibbonControlAdvHeader.GetItemParent( item ) != this );
			if( bHide )
			{
				return m_fakeHiddenItem;
			}
			else
			{
				return item;
			}
			//return ( bHide ) ? ( m_fakeHiddenItem ) : ( item );
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnQuickItemsChanged( object sender, ListItemEventArgs<ToolStripItem> e )
		{
			PerformLayout();
		}
		#endregion
	}
}
#endif
