#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;
using System.Diagnostics;

namespace Syncfusion.Windows.Forms.Tools
{
	partial class RibbonControlAdvHeader
	{
		public class QuickItemsOverflow : ToolStripOverflow
		{
			#region *** DropDownOverflowLayoutEngine
			/// <summary>
			/// Layout engine for DropDownOverflow.
			/// </summary>
			private class QuickItemsOverflowLayoutEngine : LayoutEngine
			{
				#region Methods
				
				public Size GetPreferredSize(Size proposedSize, QuickItemsOverflow parent, bool bMinWidth)
				{
					Size szResult = new Size(0, 0);

					szResult.Width += parent.Padding.Horizontal;
                    if (parent.Header != null)
                    {

					foreach (ToolStripItem item in parent.Header.OverflowsItems)
					{
						Size szItem = item.Size;

						int itemHeight = szItem.Height + item.Margin.Vertical;

						if (item is QuickItemsDropDownButton)
						{
							szResult.Width += item.Margin.Horizontal;
						}

						szResult.Width += szItem.Width;

						if (szResult.Height < itemHeight)
						{
							szResult.Height = itemHeight;
						}
					}
                    }
					szResult.Height += parent.Padding.Vertical;

					return szResult;
				}
				#endregion

				#region Overrides
				/// <summary>
				/// Lays out toolstrip items in RibbonControlAdv.
				/// </summary>
				/// <param name="container"></param>
				/// <param name="layoutEventArgs"></param>
				/// <returns></returns>
				public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
				{
					QuickItemsOverflow parent = (QuickItemsOverflow)container;

					Size displaySize = parent.DisplayRectangle.Size;

					int quickLeft = parent.Padding.Left;
					if (parent.Header != null)
					{
					if (parent.Header.RightToLeft == RightToLeft.Yes)
					{
						for (int i = parent.DisplayedItems.Count - 1; i >= 0; i--)
						{
							ToolStripItem item = parent.DisplayedItems[i] as ToolStripItem;

							if (!(item is QuickItemsDropDownButton))
							{
								ILayoutSupport owner = ( ILayoutSupport )item.Owner;
								if( owner != null )
								{
									owner.SetItemLocation( item, new Point( quickLeft, item.Margin.Top + parent.Padding.Top ) );
								}
							}
							else
							{
								QuickItemsDropDownButton quickItemsDropDown = item as QuickItemsDropDownButton;

								if (quickItemsDropDown != null)
									quickItemsDropDown.Location = new Point(quickLeft, item.Margin.Top + parent.Padding.Top);
							}

							quickLeft += item.Width;
						}
					}
					else
					{
						for ( int i = 0; i < parent.DisplayedItems.Count; i++ )
						{
							ToolStripItem item = parent.DisplayedItems[i] as ToolStripItem;

							if (!(item is QuickItemsDropDownButton))
							{
								ILayoutSupport owner = ( ILayoutSupport )item.Owner;
								if( owner != null )
								{
									owner.SetItemLocation( item, new Point( quickLeft, item.Margin.Top + parent.Padding.Top ) );
								}
							}
							else
							{
								QuickItemsDropDownButton quickItemsDropDown = item as QuickItemsDropDownButton;

								if (quickItemsDropDown != null)
									quickItemsDropDown.Location = new Point(quickLeft, item.Margin.Top + parent.Padding.Top);
							}

							quickLeft += item.Width;
						}
					}
				 }
					return parent.AutoSize;
				}
				#endregion

			}
			#endregion

			#region Initialization
			/// <summary>
			/// 
			/// </summary>
			/// <param name="ownerItem"></param>
			/// <param name="parent"></param>
			public QuickItemsOverflow(ToolStripItem ownerItem, RibbonControlAdvHeader parent)
				:base( ownerItem )
			{
				this.Header = parent;
				this.DefaultDropDownDirection = ToolStripDropDownDirection.BelowRight;
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="disposing"></param>
			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					this.Header = null;
				}
				base.Dispose(disposing);
			}
			/// <summary>
			/// 
			/// </summary>
			protected override ToolStripItemCollection DisplayedItems
			{
				get
				{
					if (Header!=null )
						return Header.OverflowsItems;
					return base.Items ;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public override LayoutEngine LayoutEngine
			{
				get
				{
					if (m_layoutEngine == null)
						m_layoutEngine = new QuickItemsOverflowLayoutEngine();

					return m_layoutEngine;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="proposedSize"></param>
			/// <returns></returns>
			public override Size GetPreferredSize(Size proposedSize)
			{
				return m_layoutEngine.GetPreferredSize(proposedSize, this, false);
			}
			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			public RibbonControlAdvHeader Header
			{
				get
				{
					return m_parent;
				}
				set
				{
					m_parent = value;
				}
			}
			#endregion

			#region Fields
			/// <summary> Instance of RibbonControlAdvLayoutEngine. </summary>
			private QuickItemsOverflowLayoutEngine m_layoutEngine;
			/// <summary></summary>
			private RibbonControlAdvHeader m_parent;
			#endregion
		}
	}
}
#endif
