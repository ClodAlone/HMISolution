#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	///
	/// </summary>
	public class PrimitivesCollectionEditor : UITypeEditor
	{
		#region members
		private IWindowsFormsEditorService m_editorService = null;
		private IServiceProvider m_serviceProvider;		
		#endregion

		#region Overrides
		public override object EditValue(ITypeDescriptorContext context,
			IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				m_editorService = ( IWindowsFormsEditorService )provider.GetService(
					typeof( IWindowsFormsEditorService ) );

				m_serviceProvider = provider;

				if( m_editorService != null)
				{
					TreeNodeAdv node = context.Instance as TreeNodeAdv;
					if( node != null )
					{
						PrimitivesEditorForm editorForm = new PrimitivesEditorForm(node.Primitives);
						if( DialogResult.OK == m_editorService.ShowDialog(editorForm) )
						{
							node.SetPrimitives( editorForm.Primitives );
						}
					}
				}
			}

			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null && context.Instance != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
		#endregion
	}
}
