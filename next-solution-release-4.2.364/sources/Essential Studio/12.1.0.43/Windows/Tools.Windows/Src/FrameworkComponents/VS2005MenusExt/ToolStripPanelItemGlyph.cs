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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	#region ToolStripPanelItemGlyph
	class ToolStripPanelItemGlyph : ToolStripExItemGlyph
	{
		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		static ToolStripPanelItemGlyph()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseGlyph"></param>
		/// <param name="glyphs"></param>
		public ToolStripPanelItemGlyph(ComponentGlyph baseGlyph, GlyphCollection glyphs):base(baseGlyph)
		{
			m_owner = glyphs;
			m_items = new GlyphCollection();
			
			if (baseGlyph != null)
			{
				ToolStripPanelItem item = baseGlyph.RelatedComponent as ToolStripPanelItem;
				if (item != null)
				{
					ToolStripItemCollection items = item.Items;
					for (int i = 0, count = items.Count; i < count; i++)
					{
						ToolStripItem it = items[i];
						if (it.Placement == ToolStripItemPlacement.Main)
						{
							ComponentGlyph glyph = DesignerUtils.GetToolStripItemGlyph(it);
							if (glyph != null)
							{
								m_items.Add(glyph);
							}
						}
					}
				}
			}
		}
		#endregion

		#region Overrides
		public override void OnInsert()
		{
			base.OnInsert();
			
			int idx = m_owner.IndexOf(this);
			if (idx >= 0)
			{
				foreach(Glyph item in m_items)
				{
					m_owner.Insert(idx, item);
				}
			}
		}
		#endregion

		#region Fields
		private GlyphCollection m_owner;
		private GlyphCollection m_items;

		#endregion
	}
	#endregion
}
#endif
