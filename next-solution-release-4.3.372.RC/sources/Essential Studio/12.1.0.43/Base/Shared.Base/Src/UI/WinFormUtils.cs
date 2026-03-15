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

using System.Windows.Forms;
using System;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Text;


namespace Syncfusion.Windows.Forms
{
	/// <internalonly/>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class WhidbeyCompatibleControlStyles
	{
		public static ControlStyles DoubleBuffer
		{
			get
			{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				return ControlStyles.OptimizedDoubleBuffer;
#else
				return ControlStyles.DoubleBuffer;
#endif
			}
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class WinFormsUtils
	{
		/// <summary>
		/// Sets / resets the specified styles on the control.
		/// </summary>
		/// <param name="control">The control on which to set / reset the styles.</param>
		/// <param name="styles">The style to set / reset.</param>
		/// <param name="setOrReset">Indicates whether to set / reset the style. True to set, False to reset.</param>
		/// <remarks>This method uses reflection to call the protected SetStyle 
		/// method on the specified control.</remarks>
		public static void ChangeStyle(Control control, ControlStyles styles, bool setOrReset)
		{
			MethodInfo mInfo = typeof(Control).GetMethod("SetStyle", 
				BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic);
			if(mInfo != null)
			{
				mInfo.Invoke(control, new object[]{ControlStyles.Selectable, false});
			}
		}
		/// <summary>
		/// Makes the specified control and all its parent controls the ActiveControl of its parent container.
		/// </summary>
		/// <param name="control">The control that needs to be made the ActiveControl of its parent.</param>
		public static bool ActivateAllParents(ContainerControl control)
		{
			if(control.Parent is ContainerControl)
				if(!ActivateAllParents(((ContainerControl)control.Parent)))
					return false;

			control.Select();

			if(!IsActiveControlInParent(control))
				return false;
			else
				return true;
		}

		public static bool IsActiveControlInParent(Control control)
		{
			if(control.Parent is ContainerControl)
			{
				ContainerControl cc = control.Parent as ContainerControl;
				return cc.ActiveControl == control;
			}
			else
				return true;
		}

		public static Control FocusedContainerControl()
		{
			Control c = Control.FromHandle(Syncfusion.Runtime.InteropServices.NativeMethods.GetFocus());
			while(c != null && !(c is ContainerControl))
				c = c.Parent;
			
			return c;
		}
		public static Control FocusedControl()
		{
			return Control.FromHandle(Syncfusion.Runtime.InteropServices.NativeMethods.GetFocus());
		}
		public static bool IsParent(Control parent, Control child)
		{
			// Parses the child's hierarchy and sees if the parent is found.
			while(child.Parent != null)
			{
				if(child.Parent == parent)
					return true;
				else
					child = child.Parent;
			}
			return false;
		}
		/// <summary>
        /// Returns the first PropertyGrid encountered in the container control's controls collection. Will recur.
		/// </summary>
		/// <param name="control">The instance to a ContainerControl.</param>
		/// <returns>A PropertyGrid instance, if found; NULL otherwise.</returns>
		public static PropertyGrid GetPropertyGridInControl(Control control)
		{
			if(control == null)
				return null;
			foreach(Control c in control.Controls)
			{
				if(c is PropertyGrid)
					return c as PropertyGrid;
				if(c is ContainerControl || c is Panel)
				{
					PropertyGrid pg = WinFormsUtils.GetPropertyGridInControl(c);
					if(pg != null)
						return pg;
				}
			}
			return null;
		}
		/// <summary>
		/// Updates the designer generated datasource full path to a different one based
		/// on the provided info.
		/// </summary>
		/// <param name="connection">The Connection object whose ConnectionString will be updated.</param>
		/// <param name="dataDirName">The directory name that will be sought after up in the exe's hierarchy (typically the "Data" dir).</param>
		/// <param name="fileName">A file in the above directory (typically an .mdb file).</param>
		/// <remarks>
		/// The designer generated path to the "Data" directory could have been changed if the user
		/// installed the product to a non-default directory.
		/// This routine will parse up the dir hierarchy from the exe file and try to find
		/// the "Data" dir and use it instead.
		/// </remarks>
		public static void UpdateDirectoryPath(System.Data.OleDb.OleDbConnection connection, 
			string dataDirName, string fileName)
		{
			int curpathStartIndex = connection.ConnectionString.IndexOf("Data Source");
			curpathStartIndex += 11; // Length of "Data Source"
			curpathStartIndex++; // Skip the ":"
			int curpathEndIndex = connection.ConnectionString.IndexOf(@";", curpathStartIndex);
			string curPath = connection.ConnectionString.Substring(curpathStartIndex, curpathEndIndex - curpathStartIndex);
			// The current path is valid, so do nothing.
			if(File.Exists(curPath))
				return;
			
			string newPath = String.Empty;
			DirectoryInfo di = new DirectoryInfo(Application.ExecutablePath).Parent;

			// Parse up the dirs looking for the data-Directory.
			while(di != null)
			{
				foreach(DirectoryInfo subDir in di.GetDirectories(dataDirName))
				{
					if(subDir.Name == dataDirName)
					{
						newPath = subDir.FullName;
						break;
					}
				}
				if(newPath != String.Empty)
					break;
				
				di = di.Parent;
			}

			if (newPath != String.Empty)
				newPath += @"\" + fileName;
			else
				newPath = FindFile(dataDirName, Path.GetFileName(fileName));

			// The data dir was found, so go ahead and replace the existing one with the new one.
			if(newPath != String.Empty)
			{
				connection.ConnectionString = connection.ConnectionString.Remove(curpathStartIndex, curpathEndIndex-curpathStartIndex);
				connection.ConnectionString = connection.ConnectionString.Insert(curpathStartIndex, newPath);
			}
		}


        /// <summary>
        /// Finds a file of the given name in the current directory or sibling "Data" directory.
        /// If file is not found, the parent folder is checked until the file is found. This method
        /// is used by our samples when they load data from a separate "Data" folder.
        /// </summary>
        /// <param name="dataDirName">The name of the "Data" folder.</param>
        /// <param name="fileName">The filename to be searched.</param>
        /// <returns>The full path of the file that was found; an empty string is returned if file is not found.</returns>
		public static string FindFile(string dataDirName, string fileName)
		{
			dataDirName = dataDirName.TrimEnd('\\', '/');
			// Check both in parent folder and Parent\Data folders.
			string dataFileName = dataDirName + "\\" + fileName;
			for (int n = 0; n < 10; n++)
			{
				if (System.IO.File.Exists(fileName))
				{
					return fileName;
				}
				if (System.IO.File.Exists(dataFileName))
				{
					return dataFileName;
				}
				fileName = @"..\" + fileName;
				dataFileName = @"..\" + dataFileName;
			}

			return "";
		}
		/// <summary>
		/// Returns the preferred size to be used for an empty cell.
		/// </summary>
		/// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
		/// <param name="font">The <see cref="Font"/> to be used.</param>
		/// <returns>The <see cref="Size"/> of the string "Wg;".</returns>
		public static Size MeasureSampleWString(Graphics g, Font font)
		{
			return g.MeasureString(MeasureEmptyCellString, font).ToSize();
		}

		static string measureWgString = "Wg;";

		/// <summary>
		/// Gets / sets the string used when doing a resize to fit for cells with empty text.
		/// </summary>
		public static string MeasureEmptyCellString
		{
			get
			{
				return measureWgString;
			}
			set
			{
				measureWgString = value;
			}
		}
	}
	/// <summary>
	/// Defines a mechanism for letting others know that you let others cancel your operation.
	/// </summary>
	/// <remarks>
	/// The <see cref="CancelListener"/> class relies on this interface.
	/// </remarks>
	public interface ICanCancel
	{
		/// <summary>
		/// To let you know that any current operation should be cancelled.
		/// </summary>
		void CancelOperation();
	}

	/// <summary>
	/// A listener class that will listen to and notify Escape key press.
	/// </summary>
	/// <remarks>
	/// As soon as you create this class, it starts listening for the Esc key press using Application.AddMessageFilter.
	/// As soon as it encounters the Escape key, it notifies the source (ICanCancel implementor) and
	/// stops listening to further Escape presses. The Release method will make it stop listening at any point.
	/// </remarks>
	public class CancelListener : IMessageFilter, IKeyboardProcHookClient
	{
		ICanCancel canCancel;
		/// <summary>
		/// Creates a new instance of the CancelListener class.
		/// </summary>
		/// <param name="canCancel">The instance that will be notified on Escape key press.</param>
		public CancelListener(ICanCancel canCancel)
		{
			this.canCancel = canCancel;
			MessageFilterEntryHelper.AddMessageFilter(this, false);
		}

		// This will be called in a "pure .net app".
		bool IMessageFilter.PreFilterMessage(ref Message m)
		{
			if(m.Msg >= 0x100/*WM_KEYDOWN*/)
			{
				Keys keys = (Keys)m.WParam.ToInt32();
				if(keys == Keys.Escape)	
				{
					this.canCancel.CancelOperation();
					MessageFilterEntryHelper.RemoveMessageFilter(this);
					return true;
				}
			}
			return false;
		}

		// This will be called when hosted in a native app.
		bool IKeyboardProcHookClient.KeyboardHookProc(int wParam, int lParam)
		{
			// If key down
			if((lParam & 0x80000000) == 0)
			{
				Keys keys = (Keys)wParam;
				if(keys == Keys.Escape)	
				{
					this.canCancel.CancelOperation();
					MessageFilterEntryHelper.RemoveMessageFilter(this);
					return true;
				}
			}
			
			return false;
		}

		/// <summary>
		/// Makes this instance stop listening for Escape key press.
		/// </summary>
		public void Release()
		{
			MessageFilterEntryHelper.RemoveMessageFilter(this);
		}
	}
	[Documentation.DocumentationExclude()]
	public interface ITabbedMDIManager
	{
		Form GetMDIParent();
		bool ProcessCmdKey(ref Message msg, Keys keyData);
	}
	[Documentation.DocumentationExclude()]
	public interface ITabHost
	{
	}
	[Documentation.DocumentationExclude()]
	public interface ISplitterHost
	{
	}
	[Documentation.DocumentationExclude()]
	public class ImageUtil
	{
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public static Image ConvertToImage(byte[] rawData)
		{
			if (rawData != null)
			{
				// Check for OLE - Paintbrush picture.
				Stream buffer = GetBitmapStream(rawData);

				// Regular picture.
				if (buffer == null)
					buffer = new MemoryStream(rawData);

				return Image.FromStream(buffer, true);
			}
			return null;
		}
		private static Stream GetBitmapStream(byte[] rawData)
		{
			// 0x1C15 = OLEContainer signature
			if (rawData[0] == 0x15 && rawData[1] == 0x1C)
			{
				// Make sure the class of the object is a Paint Brush object:
				string s = Encoding.ASCII.GetString(rawData, 47 + 12, 6);

				if (s != "PBrush")
					return null;

				try
				{
					// Extract bitmap data from container.
					return new MemoryStream(rawData, 78, rawData.Length - 78);
				}
				catch
				{
					// Fill with error code.
				}
			}
			else
			{
				// Fill with error code.
			}
			return null;
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		public static Image ConvertToImage(object value)
		{
			byte[] rawData = value as byte[];
			if (rawData != null)
				return ConvertToImage(rawData);

			return value as Image;
		}

	}
}