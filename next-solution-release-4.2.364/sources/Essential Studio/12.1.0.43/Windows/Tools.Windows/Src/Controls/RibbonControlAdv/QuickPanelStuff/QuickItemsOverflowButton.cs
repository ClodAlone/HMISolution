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
using Syncfusion.Windows.Forms.Collections;

namespace Syncfusion.Windows.Forms.Tools
{
	partial class RibbonControlAdvHeader
	{
		/// <summary>
		/// Represents additional functionality for quick items. Always exists at the end of quick items panel.
		/// </summary>
		internal class QuickItemsOverflowButton : ToolStripDropDownButton, ICustomItem
		{
			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="disposing"></param>
			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					this.DropDown.Dispose();
					this.DropDown = null;
				}
				base.Dispose(disposing);
			}
			/// <summary> Fills dropdown menu. </summary>
			/// <param name="e"></param>
			protected override void OnPaint(PaintEventArgs e)
			{
				if (this.Parent != null)
				{
					this.Parent.Renderer.DrawOverflowButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
				}
			}
			/// <summary> Initializes dropdown. </summary>
			/// <param name="e"></param>
			protected override void OnDropDownShow(EventArgs e)
			{
				this.DropDown.PerformLayout();
				base.OnDropDownShow(e);
			}
			/// <summary> Indicates if DropDown has items. 
			/// In our situation return true.</summary>
			public override bool HasDropDownItems
			{
				get
				{
					return true;
				}
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
					RibbonControlAdvHeader header = this.Parent as RibbonControlAdvHeader;

					if (header == null) throw new Exception("QuickItemsOverflowButton can be owned by RibbonControlAdvHeader only.");

					return header;
				}
			}
			#endregion

			#region Initialization
			/// <summary>
			/// Creates new instance of QuickItemsOverflowButton.
			/// </summary>
			public QuickItemsOverflowButton(RibbonControlAdvHeader parent)
			{
				this.DropDown = new QuickItemsOverflow(this, parent);
				this.Parent = parent;
				this.DisplayStyle = ToolStripItemDisplayStyle.None;
				this.ShowDropDownArrow = true;
				this.Padding = new Padding(0, 4, 0, 4);
				this.Margin = new Padding(1);
			}
			#endregion

			#region ICustomItem Members
			/// <summary>
			/// 
			/// </summary>
			ToolStrip ICustomItem.Owner
			{
				get { return this.Parent; }
			}
			/// <summary>
			/// 
			/// </summary>
			ToolStripItemPlacement ICustomItem.Placement
			{
				get { return this.Parent != null ? ToolStripItemPlacement.Main : ToolStripItemPlacement.None; }
			}

			#endregion
		}
	}
}
#endif
