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
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	#region ToolStripItemsEditor
    /// <exclude/>
	public class ToolStripItemsEditor : UITypeEditor
	{
		#region *** Context
		class Context : CustomContext
		{
			#region Constructors
			/// <summary>
			/// 
			/// </summary>
			/// <param name="baseContext"></param>
			public Context(ITypeDescriptorContext baseContext)
				: base(baseContext)
			{
			}
			#endregion

			#region Properties
			protected override object Instance
			{
				get
				{
					object instance = null;

					if (this.BaseContext != null)
					{
						ToolStripPanelItem panelItem = this.BaseContext.Instance as ToolStripPanelItem;
						
						if (panelItem!=null)
						{
							instance = panelItem.Control;
						}
						else instance = this.BaseContext.Instance;
					}

					return instance;
				}
			}
			#endregion
		}
		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		static ToolStripItemsEditor()
		{
			Type edType = Type.GetType("System.Windows.Forms.Design.ToolStripCollectionEditor, System.Design");

			if (edType != null)
			{
				m_ToolStripCollectionEditor = Activator.CreateInstance(edType, false) as CollectionEditor;
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			object obj = null;

			if (m_ToolStripCollectionEditor != null)
			{
				obj = m_ToolStripCollectionEditor.EditValue(new Context(context), provider, value);
			}
			else obj = base.EditValue(context, provider, value);

			return obj;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (m_ToolStripCollectionEditor != null)
			{
				return m_ToolStripCollectionEditor.GetEditStyle(context);
			}
			return base.GetEditStyle(context);
		}
		#endregion

		#region Fields
		static CollectionEditor m_ToolStripCollectionEditor;
		#endregion
	}
	#endregion
}
#endif