#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.ComponentModel;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Summary description for GradientPanelExtCollectionEditor.
	/// </summary>
	public class GradientPanelExtCollectionEditor : UITypeEditor
	{
		#region Class Overrides

		public override object EditValue( ITypeDescriptorContext context, 
			IServiceProvider provider, object value )
		{
			if ( context != null && context.Instance != null && provider != null )
			{
				IWindowsFormsEditorService edSvc = ( IWindowsFormsEditorService )provider.GetService( typeof( IWindowsFormsEditorService ) );
				IServiceProvider serviceProvider = provider;

				if( edSvc != null )
				{
					if( context.Instance is GradientPanelExt )
					{
						GradienPanelExtCollectioEditorForm form = new GradienPanelExtCollectioEditorForm( context.Instance as GradientPanelExt, provider );
						edSvc.ShowDialog( form );
					}
				}
			}

			return value;
		}


		public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
		{
			if( context != null && context.Instance != null )
			{
				return UITypeEditorEditStyle.Modal;
			}

			return base.GetEditStyle( context );
		}


		#endregion
	}
}
