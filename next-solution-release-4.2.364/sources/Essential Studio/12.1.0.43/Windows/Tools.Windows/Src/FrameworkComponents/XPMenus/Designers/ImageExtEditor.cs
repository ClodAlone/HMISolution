#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// Represents TypeEditor for <see cref="ImageExt"/>.
	/// </summary>
	public class ImageExtEditor : UITypeEditor
	{
		#region Class Cnstants

		/// <summary>
		/// Extension for icon file.
		/// </summary>
		private const string DEF_ICON_EXTENSION = ".ico";

		#endregion

		#region Class Overrides

		public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, 
			object value )
		{
			if ( context != null && context.Instance != null && provider != null )
			{
				IWindowsFormsEditorService edSvc = ( IWindowsFormsEditorService )provider.GetService( typeof( IWindowsFormsEditorService ) );
				IServiceProvider serviceProvider = provider;

				if( edSvc != null )
				{
					if( context.Instance is BarItem )
					{
						// initializes OpenFileDialog
						OpenFileDialog dlg = new OpenFileDialog();
						dlg.Filter = GetFilterString();

						// shows dialog
						if( dlg.ShowDialog() == DialogResult.OK )
						{
							int currentSelectIndex = dlg.FilterIndex;
							string extension = System.IO.Path.GetExtension( dlg.FileName ).ToLower();

							FileStream stream = new FileStream( dlg.FileName, 
								FileMode.Open, FileAccess.Read, FileShare.Read );

							ImageExt image = null;

							// loads bitmap or icon from stream
							image = ( extension == DEF_ICON_EXTENSION ) ? LoadIcon( stream ) : image = LoadImage( stream );

							value = image;
						}
					}
				}
			}

			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if( context != null && context.Instance != null )
			{
				return UITypeEditorEditStyle.Modal;
			}

			return base.GetEditStyle( context );
		}



		#endregion

		#region Class Utiliti Methods

		/// <summary>
		/// Loads icon from stream.
		/// </summary>
		protected virtual Icon LoadIcon( Stream stream )
		{
			return new Icon( stream );
		}

		/// <summary>
		/// Loads image from stream.
		/// </summary>
		protected virtual Bitmap LoadImage( Stream stream )
		{
			return new Bitmap( stream );
		}

		/// <summary>
		/// Gets filter string for OpenDialog.
		/// </summary>
		private string GetFilterString()
		{
			string filter = "Image files (*.bmp, *.gif, *.jpg, *.jpeg, *.png, *.ico)|*.bmp; *.gif; *.jpg; *.jpeg; *.png; *.ico";
			
			return filter;
		}

		#endregion
	}
}
