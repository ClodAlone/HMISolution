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

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Tools
{
	internal class MDITabsDragCursors
	{
		[ThreadStatic()]
		public static Cursor dropCursor = null;
		[ThreadStatic()]
		public static Cursor nodropCursor = null;

		protected static Cursor GetCursor(string resourceName)  
		{
			Cursor cursor = null;
			try
			{
				Type type = typeof(MDITabsDragCursors);
				Assembly assembly = type.Module.Assembly;
//				string[] resourceNames = assembly.GetManifestResourceNames();
				Stream stream = assembly.GetManifestResourceStream(resourceName);//cursorNS + cursorName
				cursor = new Cursor(stream);
			}  
			catch(System.Exception exception)
			{
				MessageBox.Show(exception.Message);
				throw exception;
			}  

			return cursor;
		}

		public static Cursor DropCursor
		{
			get
			{
				if (MDITabsDragCursors.dropCursor == null)
					MDITabsDragCursors.dropCursor = GetCursor(@"Syncfusion.Windows.Forms.Tools.FrameworkComponents.TabbedMDI.Cursors.Drop.CUR");
				return MDITabsDragCursors.dropCursor;
			}
		}
		public static Cursor NodropCursor
		{
			get
			{
				if (MDITabsDragCursors.nodropCursor == null)
					MDITabsDragCursors.nodropCursor = GetCursor(@"Syncfusion.Windows.Forms.Tools.FrameworkComponents.TabbedMDI.Cursors.NoDrop.CUR");
				return MDITabsDragCursors.nodropCursor;
			}
		}
	}
}
