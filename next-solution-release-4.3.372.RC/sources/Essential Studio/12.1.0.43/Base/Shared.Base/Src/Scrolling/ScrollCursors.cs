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

namespace Syncfusion.Windows.Forms
{
	[Syncfusion.Documentation.DocumentationExclude()]
	class ScrollCursors
	{
		[ThreadStatic] static string cursorNS = @"Syncfusion.Windows.Forms.Scrolling.";

		[ThreadStatic] static Cursor dragWheelAllCursor = null;
		[ThreadStatic] static Cursor dragWheelSouthCursor = null;
		[ThreadStatic] static Cursor dragWheelNorthCursor = null;
		[ThreadStatic] static Cursor dragWheelWestCursor = null;
		[ThreadStatic] static Cursor dragWheelEastCursor = null;

		protected static Cursor GetCursor(string cursorName)  
		{
			Cursor cursor = null;

			try
			{
				Type type = typeof(ScrollCursors);
				Stream stream = type.Module.Assembly.GetManifestResourceStream(cursorNS + cursorName);
				cursor = new Cursor(stream);
			}  
			catch(System.Exception exception)
			{
				MessageBox.Show(exception.Message);
				throw;
			}  

			return cursor;
		}

		public static Cursor DragWheelAllCursor
		{
			get
			{
				if (ScrollCursors.dragWheelAllCursor == null)
					ScrollCursors.dragWheelAllCursor = GetCursor(@"IMA.CUR");
				return ScrollCursors.dragWheelAllCursor;
			}
		}

		public static Cursor DragWheelSouthCursor
		{
			get
			{
				if (ScrollCursors.dragWheelSouthCursor == null)
					ScrollCursors.dragWheelSouthCursor = GetCursor(@"IMD.CUR");
				return ScrollCursors.dragWheelSouthCursor;
			}
		}

		public static Cursor DragWheelNorthCursor
		{
			get
			{
				if (ScrollCursors.dragWheelNorthCursor == null)
					ScrollCursors.dragWheelNorthCursor = GetCursor(@"IMU.CUR");
				return ScrollCursors.dragWheelNorthCursor;
			}
		}

		public static Cursor DragWheelWestCursor
		{
			get
			{
				if (ScrollCursors.dragWheelWestCursor == null)
					ScrollCursors.dragWheelWestCursor = GetCursor(@"IML.CUR");
				return ScrollCursors.dragWheelWestCursor;
			}
		}

		public static Cursor DragWheelEastCursor
		{
			get
			{
				if (ScrollCursors.dragWheelEastCursor == null)
					ScrollCursors.dragWheelEastCursor = GetCursor(@"IMR.CUR");
				return ScrollCursors.dragWheelEastCursor;
			}
		}
	}
}
