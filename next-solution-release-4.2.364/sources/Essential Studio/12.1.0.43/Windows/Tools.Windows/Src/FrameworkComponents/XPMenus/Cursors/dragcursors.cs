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

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// Summary description for Cursors.
	/// </summary>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DragCursors
	{
//		static string cursorNS = @"Syncfusion.Windows.Forms.Cursors.";

		[ThreadStatic()]
		private static Cursor dragCursor = null;
		[ThreadStatic()]
		private static Cursor copyCursor = null;
		[ThreadStatic()]
		private static Cursor nodropCursor = null;

		protected static Cursor GetCursor(string resourceName)  
		{
			Cursor cursor = null;
			try
			{
				Type type = typeof(DragCursors);
				Assembly assembly = type.Module.Assembly;
				string[] resourceNames = assembly.GetManifestResourceNames();
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

		public static Cursor DragCursor
		{
			get
			{
				if (DragCursors.dragCursor == null)
					DragCursors.dragCursor = GetCursor(@"Syncfusion.Windows.Forms.Tools.FrameworkComponents.XPMenus.Cursors.DragCursor.cur");
				return DragCursors.dragCursor;
			}
		}

		public static Cursor CopyCursor
		{
			get
			{
				if (DragCursors.copyCursor == null)
					DragCursors.copyCursor = GetCursor(@"Syncfusion.Windows.Forms.Tools.FrameworkComponents.XPMenus.Cursors.CopyCursor.cur");
				return DragCursors.copyCursor;
			}
		}

		public static Cursor NodropCursor
		{
			get
			{
				if (DragCursors.nodropCursor == null)
					DragCursors.nodropCursor = GetCursor(@"Syncfusion.Windows.Forms.Tools.FrameworkComponents.XPMenus.Cursors.NoDropCursor.cur");
				return DragCursors.nodropCursor;
			}
		}
	}
}
