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
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	#region MenuButtonTypeItem
	class MenuButtonTypeItem : ToolStripMenuItem
	{
		#region Constructors
		public MenuButtonTypeItem(Type itemType)
		{
			m_itemType = itemType;
		}
		#endregion

		#region Methods
		public ToolStripItem CreateItem(IServiceProvider serviceProvider)
		{
			ToolStripItem item = null;

			if (serviceProvider != null)
			{
				IDesignerHost host = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if (host != null)
				{
					item = Activator.CreateInstance(m_itemType) as ToolStripItem;
					if (item != null)
					{
						if (!(item is ToolStripControlHost))
						{
							item.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
							item.Image = new System.Drawing.Bitmap(typeof(ToolStripButton), "blank.bmp");
						}

						host.Container.Add(item);
						item.Text = item.Name;
					}
				}
			}
			return item;
		}
		#endregion

		#region Overrides
		public override string ToString()
		{
			return base.ToString();
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public override string Text
		{
			get
			{
				if (m_itemType != null)
				{
					return m_itemType.Name;
				}
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}
		/// <summary>
		/// Array of standard items
		/// </summary>
		public static MenuButtonTypeItem[] Items
		{
			get
			{
				return new MenuButtonTypeItem[] {
					new MenuButtonTypeItem(typeof(ToolStripButton)),
					new MenuButtonTypeItem(typeof(ToolStripSplitButton)),
					new MenuButtonTypeItem(typeof(ToolStripDropDownButton)),
					new MenuButtonTypeItem(typeof(ToolStripMenuItem)),
					new MenuButtonTypeItem(typeof(ToolStripPanelItem)),
					new MenuButtonTypeItem(typeof(ToolStripComboBox)),
					new MenuButtonTypeItem(typeof(ToolStripTextBox)),
				};
			}
		}
		#endregion

		#region Fields
		Type m_itemType;
		#endregion

	}
	#endregion
}
#endif
