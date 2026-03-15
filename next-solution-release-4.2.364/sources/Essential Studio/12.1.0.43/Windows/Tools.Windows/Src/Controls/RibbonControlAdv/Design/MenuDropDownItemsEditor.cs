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
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	#region MenuDropDownItemsEditor
	class MenuDropDownItemsEditor : UITypeEditor
	{
		#region Constructors
		public MenuDropDownItemsEditor()
		{
			m_baseEditor = TypeDescriptor.GetEditor(typeof(ToolStripItemCollection), typeof(UITypeEditor)) as UITypeEditor;
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return m_baseEditor.GetEditStyle(context);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="provider"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			object obj = null;
			if (value != null)
			{
				TypeDescriptionProvider tdOfficeButton = AddProvider(typeof(OfficeButton));
				TypeDescriptionProvider tdOfficeDropDownButton = AddProvider(typeof(OfficeDropDownButton));
				TypeDescriptionProvider tdOfficeOfficeSplitButton = AddProvider(typeof(OfficeSplitButton));

				obj = m_baseEditor.EditValue(new Context(context), provider, value);

				TypeDescriptor.RemoveProvider(tdOfficeButton, typeof(OfficeButton));
				TypeDescriptor.RemoveProvider(tdOfficeDropDownButton, typeof(OfficeDropDownButton));
				TypeDescriptor.RemoveProvider(tdOfficeOfficeSplitButton, typeof(OfficeSplitButton));
			}
			return obj;
		}
		#endregion

		#region
		private TypeDescriptionProvider AddProvider(Type type)
		{
			TypeDescriptionProvider provider = new OfficeButtonTypeDescriptionProvider(type);
			
			TypeDescriptor.AddProvider(provider, type);

			return provider;
		}

		#endregion

		#region Fields
		UITypeEditor m_baseEditor;
		#endregion

		#region *** Context
		class Context : CustomContext
		{
			#region Constructors
			public Context(ITypeDescriptorContext baseContext) : base(baseContext)
			{ 
			}
			#endregion

			#region Properties
			protected override object Instance
			{
				get
				{
					object instance = this.BaseContext.Instance;
					
					MenuDropDown.Panel panel = instance as MenuDropDown.Panel;
					if (panel != null)
					{
						return panel.ToolStrip;
					}

					return instance;
				}
			}
			#endregion
		}
		#endregion
	}
	#endregion
}
#endif