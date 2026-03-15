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
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms.Tools
{
	partial class RibbonControlAdvHeader
	{
		/// <summary>
		/// Represents additional functionality for quick items. Always exists at the end of quick items panel.
		/// </summary>
		internal class QuickItemsDropDownButton : ToolStripDropDownButton, ICustomItem
		{
			#region Internal Classes
			/// <summary>
			/// 
			/// </summary>
			private class QuickItemsDropDownMenu
				: ContextMenuStripEx
			{
				#region Fields
				/// <summary>
				/// 
				/// </summary>
				private RibbonControlAdvHeader m_header;
				#endregion

				#region Initialization
				/// <summary>
				/// 
				/// </summary>
				/// <param name="header"></param>
				public QuickItemsDropDownMenu( RibbonControlAdvHeader header )
				{
					m_header = header;
					this.Renderer = m_header.Renderer;
				}
				#endregion

				#region Overrides
				/// <summary>
				/// 
				/// </summary>
				/// <param name="disposing"></param>
				protected override void Dispose(bool disposing)
				{
					m_header = null;
					this.Renderer = null;

					base.Dispose(disposing);
				}
				/// <summary>
				/// 
				/// </summary>
				/// <param name="e"></param>
				protected override void OnOpening( CancelEventArgs e )
				{
					if( m_header != null )
					{
						if (this.Renderer != m_header.Renderer)
						{
							this.Renderer = m_header.Renderer;
						}
					}

					base.OnOpening( e );
				}
				#endregion
			}
			#endregion

			#region Properties
			/// <summary>
			/// Gets or Sets coordinates of the upper-left corner
			/// </summary>
			public Point Location
			{
				get { return this.Bounds.Location; }
				set { SetBounds(new Rectangle(value, this.Size)); }
			}
			/// <summary>
			/// Returns owner casted to RibbonControlAdvHeader.
			/// </summary>
			private RibbonControlAdvHeader ParentHeader
			{
				get
				{
					return m_header;
				}
			}
			#endregion

			#region Initialization
			/// <summary>
			/// Creates new instance of QuickItemsDropDownButton.
			/// </summary>
			public QuickItemsDropDownButton(RibbonControlAdvHeader header)
			{
				m_header = header;

				this.DisplayStyle = ToolStripItemDisplayStyle.None;
				this.ShowDropDownArrow = true;
				this.Padding = new Padding( 0, 4, 1, 4 );
				this.Margin = new Padding(1);
			}
			#endregion

			#region ICustomItem Members
			/// <summary>
			/// 
			/// </summary>
			ToolStrip ICustomItem.Owner
			{
				get
				{
					return this.Parent==null ? m_header:this.Parent;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			ToolStripItemPlacement ICustomItem.Placement
			{
				get { return ToolStripItemPlacement.Main; }
			}
			#endregion

			#region Event Handlers
			/// <summary>
			/// Shows customization dialog.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void OnCustomizeMenuItemClick( object sender, EventArgs e )
			{
                RibbonControlAdv ribbon = m_header.Parent as RibbonControlAdv;
                if (ribbon.RibbonStyle == RibbonStyle.Office2007 || ribbon.RibbonStyle == RibbonStyle.Office2010 || ribbon.Show2010CustomizeQuickItemDialog)
                {
                    CustomizeQuickItemsDialog.Execute(this.ParentHeader);
                   
                }
                else
                    Office2013CustomizeQuickItemsDialog.Execute(this.ParentHeader);
			}
			/// <summary>
			/// Shows or hides quick item referenced by clicked menu item.
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void OnQuickMenuItemClick( object sender, EventArgs e )
			{
				ToolStripMenuItem menuItem = ( ToolStripMenuItem )sender;

				Component item = menuItem.Tag as Component;

				if( item != null )
				{
					if( menuItem.Checked )
					{
						this.ParentHeader.Items.Remove(item as ToolStripItem);
					}
					else
					{
						this.ParentHeader.AddQuickItem(QuickToolstripReflectable.GetItemToReflect(item));
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void OnPlaceBelowRibbonMenuItemClick( object sender, EventArgs e )
			{
				RibbonControlAdv ribbon = m_header.Parent as RibbonControlAdv;
				if( ribbon != null )
				{
					ribbon.ShowQuickPanelBelowRibbon = !ribbon.ShowQuickPanelBelowRibbon;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			private void OnMinimizeRibbonMenuCheckedChanged( object sender, EventArgs e )
			{
				ToolStripMenuItem item = sender as ToolStripMenuItem;
				if( item != null )
				{
					RibbonControlAdv ribbonControl = m_header.Parent as RibbonControlAdv;
					if( ribbonControl != null )
					{
						ribbonControl.MinimizePanel = item.Checked;
					}
				}
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="disposing"></param>
			protected override void Dispose(bool disposing)
			{
				m_header = null;
				base.Dispose(disposing);
			}
			/// <summary>
			/// Fills dropdown menu.
			/// </summary>
			/// <param name="e"></param>
			protected override void OnPaint(PaintEventArgs e)
			{
				if (m_header != null)
				{
                    if (m_header.Renderer is Office2010RibbonHeaderRenderer)
                    {
                        if (m_header.BackStageView != null && m_header.BackStageView.BackStage!=null )
                            if(m_header .BackStageView.BackStage.Visible )
                        (m_header.Renderer as Office2010RibbonHeaderRenderer).IsEnable = false;
                            else
                                (m_header.Renderer as Office2010RibbonHeaderRenderer).IsEnable = true;
                        

                    }
                    else if (m_header.Renderer is Office2013RibbonHeaderRenderer)
                    {
                        if (m_header.BackStageView != null && m_header.BackStageView.BackStage != null)
                            if (m_header.BackStageView.BackStage.Visible)
                                (m_header.Renderer as Office2013RibbonHeaderRenderer).IsEnable = false;
                            else
                                (m_header.Renderer as Office2013RibbonHeaderRenderer).IsEnable = true;
                    }
					m_header.Renderer.DrawDropDownButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
				}
			}
			/// <summary>
			/// Initializes dropdown.
			/// </summary>
			/// <param name="e"></param>
			protected override void OnDropDownShow(EventArgs e)
			{
				ToolStripDropDown dropdown = this.DropDown;
				
				dropdown.Items.Clear();

				RibbonControlAdv ribbonControl = m_header.Parent as RibbonControlAdv;
				if (ribbonControl != null)
				{
					List<Component> itemsToShow = ribbonControl.ItemsToShowInQuickMenu;

					if (itemsToShow.Count > 0)
					{
						Dictionary<Component, IQuickItem> reflectedItems = GetReflectedItems();

						foreach (Component c in itemsToShow)
						{
							ToolStripMenuItem menuItem = new ToolStripMenuItem(this.ParentHeader.GetText(c), null, OnQuickMenuItemClick);

							if (reflectedItems.ContainsKey(c))
							{
								menuItem.Checked = true;
								menuItem.Tag = reflectedItems[c];
							}
							else
							{
								menuItem.Checked = false;
								menuItem.Tag = c;
							}

							dropdown.Items.Add(menuItem);
						}
						
						dropdown.Items.Add(new ToolStripSeparator());
					}
					dropdown.Items.Add(new ToolStripMenuItem(ribbonControl.SystemText.QuickAccessCustomizeMenuText, null, OnCustomizeMenuItemClick));
					string placeBelowText = ( ribbonControl.ShowQuickPanelBelowRibbon ) ?
						( ribbonControl.SystemText.QuickAccessPlaceAboveText ) : ( ribbonControl.SystemText.QuickAccessPlaceBelowText );
					dropdown.Items.Add( new ToolStripMenuItem( placeBelowText, null, OnPlaceBelowRibbonMenuItemClick ) );
					dropdown.Items.Add( new ToolStripSeparator() );
					
					ToolStripMenuItem menuMinimize = new ToolStripMenuItem( ribbonControl.SystemText.QuickAccessMinimizeRibbon );
					menuMinimize.Enabled = ribbonControl.AllowCollapse;
					menuMinimize.Checked = ribbonControl.MinimizePanel;
					menuMinimize.CheckOnClick = true;
					menuMinimize.CheckedChanged += new EventHandler( OnMinimizeRibbonMenuCheckedChanged );
					dropdown.Items.Add( menuMinimize );

					dropdown.Text = ribbonControl.SystemText.QuickAccessCustomizeCaptionText;
				}
				
				base.OnDropDownShow(e);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			protected override ToolStripDropDown CreateDefaultDropDown()
			{
				return new QuickItemsDropDownMenu( m_header );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnMouseDown( MouseEventArgs e )
			{
				base.OnMouseDown( e );

				if( e.Button == MouseButtons.Right )
				{
					ShowDropDown();
				}
			}
			#endregion

			#region Implementation
			/// <summary>
			/// Gets collection of reflected items from quick panel.
			/// </summary>
			/// <returns>List of reflected items from quick panel.</returns>
			private Dictionary<Component, IQuickItem> GetReflectedItems()
			{
				Dictionary<Component, IQuickItem> result = new Dictionary<Component, IQuickItem>();

				foreach (ToolStripItem item in this.ParentHeader.QuickItems)
				{
					IQuickItem quickItem = item as IQuickItem;

					if (quickItem != null)
					{
						result.Add(quickItem.ReflectedComponent, quickItem);
					}
				}

				return result;
			}
			#endregion

			#region Fields
			RibbonControlAdvHeader m_header;
			#endregion
		}
	}
}
#endif

