#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
//  Author: Jeff Boenig
//
#endregion

using System;
using System.Windows.Forms;
using System.IO;

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Provides access to Essential Diagram resources.
	/// </summary>
	public class Resources
	{
//		private static System.Resources.ResourceManager stringMgr = new System.Resources.ResourceManager("Syncfusion.Windows.Forms.Diagram.Resources.Strings", typeof(Syncfusion.Windows.Forms.Diagram.Resources).Assembly);
//		private static System.Resources.ResourceManager textureMgr = new System.Resources.ResourceManager("Syncfusion.Windows.Forms.Diagram.Resources.Textures", typeof(Syncfusion.Windows.Forms.Diagram.Resources).Assembly);
		private static System.Resources.ResourceManager stringMgr = new System.Resources.ResourceManager("Syncfusion.Diagram.Base.Resources.Strings", typeof(Resources).Assembly);
		private static System.Resources.ResourceManager textureMgr = new System.Resources.ResourceManager("Syncfusion.Diagram.Base.Resources.Textures", typeof(Resources).Assembly);


		/// <summary>
		/// Provides access to resource strings.
		/// </summary>
		public class Strings
		{
			/// <summary>
			/// Provides access to PropertyNames strings in the resource file.
			/// </summary>
			public class PropertyNames
			{
                /// <summary>
                /// Gets the specified property name ID.
                /// </summary>
                /// <param name="propertyNameID">The property name ID.</param>
                /// <returns></returns>
				public static string Get( string propertyNameID )
				{
					return Resources.stringMgr.GetString( "PropertyName." + propertyNameID );
				}
			}
			/// <summary>
			/// Provides access to Toolname strings in the resource file.
			/// </summary>
			public class Toolnames
			{
				/// <summary>
				/// Returns a toolname based on a resource ID.
				/// </summary>
				/// <param name="toolResID">Resource ID of the tool</param>
				/// <returns>Toolname string</returns>
				public static string Get(string toolResID)
				{
					return Resources.stringMgr.GetString("Toolnames." + toolResID);
				}
			}
			/// <summary>
			/// Provides access to object name strings in the resource file.
			/// </summary>
			public class ObjectName
			{
				/// <summary>
				/// Returns an object name string based on a resource ID.
				/// </summary>
				/// <param name="objNameResID">Resource ID of the object name</param>
				/// <returns>Object name string</returns>
				public static string Get(string objNameResID)
				{
					return Resources.stringMgr.GetString("ObjectName." + objNameResID);
				}
			}
			/// <summary>
			/// Provides access to command description strings in the resource file.
			/// </summary>
			public class CommandDescriptions
			{
				/// <summary>
				/// Returns a command description given the name of a command.
				/// </summary>
				/// <param name="cmdName">Name of the command to return description for</param>
				/// <returns>String description of a command</returns>
				public static string Get(string cmdName)
				{
					return Resources.stringMgr.GetString("CommandDescription." + cmdName);
				}
			}
			/// <summary>
			/// Provides access to message strings in the resource file.
			/// </summary>
			public class Messages
			{
				/// <summary>
				/// Returns a message given an ID.
				/// </summary>
				/// <param name="msgID">ID of the message to return</param>
				/// <returns>Message string</returns>
				public static string Get(string msgID)
				{
					return Resources.stringMgr.GetString("Message." + msgID);
				}
			}
			/// <summary>
			/// Provides access to caption strings in the resource file.
			/// </summary>
			public class Captions
			{
				/// <summary>
				/// Returns a caption given an ID.
				/// </summary>
				/// <param name="captionID">ID of the caption to return</param>
				/// <returns>Message string</returns>
				public static string Get(string captionID)
				{
					return Resources.stringMgr.GetString("Caption." + captionID);
				}
			}
		}
		/// <summary>
		/// Provides access to cursors.
		/// </summary>
		public class Cursors
		{
			/// <summary>
			/// Path to assembly resources.
			/// </summary>
			/// <remarks>
			/// Can't be set as [ThreadStaticAttribute]. Please refer to MSDN
			/// "ThreadStaticAttribute Class" page at Remarks->Note.
			/// </remarks>
			readonly static string cursorNS = @"Syncfusion.Diagram.Base.Resources.";
			[ThreadStaticAttribute]
			static Cursor cursorPanReady = null;
			[ThreadStaticAttribute]
			static Cursor cursorPanning = null;
			[ThreadStaticAttribute]
			static Cursor cursorZoom = null;
            [ThreadStaticAttribute]
            static Cursor cursorPort = null;
			[ThreadStaticAttribute]
			static Cursor cursorConnect = null;
			[ThreadStaticAttribute]
			static Cursor cursorRotateReady = null;
			[ThreadStaticAttribute]
			static Cursor cursorRotate = null;
			[ThreadStaticAttribute]
			static Cursor cursorInsertVertex = null;
			[ThreadStaticAttribute]
			static Cursor cursorEditVertex = null;
			[ThreadStaticAttribute]
			static Cursor cursorDeleteVertex = null;
            [ThreadStaticAttribute]
            static Cursor cursorPencil = null;
			/// <summary>
			/// Returns the cursor matching the name passed in.
			/// </summary>
			/// <param name="cursorName">Name of cursor to return</param>
			/// <returns>Cursor object</returns>
			protected static Cursor GetCursor(string cursorName)
			{
				Cursor cursor = null;

				try
				{
					Type type = typeof(Resources);
					Stream stream = type.Module.Assembly.GetManifestResourceStream(cursorNS + cursorName);
					cursor = new Cursor(stream);
				}
				catch(System.Exception exception)
				{
					MessageBox.Show(exception.Message);
					throw exception;
				}  

				return cursor;
			}

			/// <summary>
			/// Returns the Pan ready cursor.
			/// </summary>
			public static Cursor PanReady
			{
				get
				{
					if (Cursors.cursorPanReady == null)
					{
						Cursors.cursorPanReady = GetCursor(@"PanReady.cur");
					}
					return Cursors.cursorPanReady;
				}
			}

			/// <summary>
			/// Returns the Panning cursor.
			/// </summary>
			public static Cursor Panning
			{
				get
				{
					if (Cursors.cursorPanning == null)
					{
						Cursors.cursorPanning = GetCursor(@"Panning.cur");
					}
					return Cursors.cursorPanning;
				}
			}

			/// <summary>
			/// Returns the Zoom cursor.
			/// </summary>
			public static Cursor Zoom
			{
				get
				{
					if (Cursors.cursorZoom == null)
					{
						Cursors.cursorZoom = GetCursor(@"Zoom.cur");
					}
					return Cursors.cursorZoom;
				}
			}

            /// <summary>
            /// Returns the Port cursor.
            /// </summary>
            public static Cursor Port
            {
                get
                {
                    if (Cursors.cursorPort == null)
                    {
                        Cursors.cursorPort = GetCursor(@"PortInsert.cur");
                    }
                    return Cursors.cursorPort;
                }
            }

			/// <summary>
			/// Returns the Connect cursor.
			/// </summary>
			public static Cursor Connect
			{
				get
				{
					if (Cursors.cursorConnect == null)
					{
						Cursors.cursorConnect = GetCursor(@"Connect.cur");
					}
					return Cursors.cursorConnect;
				}
			}

			/// <summary>
			/// Returns the RotateReady cursor.
			/// </summary>
			public static Cursor RotateReady
			{
				get
				{
					if (Cursors.cursorRotateReady == null)
					{
						Cursors.cursorRotateReady = GetCursor(@"RotateReady.cur");
					}
					return Cursors.cursorRotateReady;
				}
			}

			/// <summary>
			/// Returns the Rotate cursor.
			/// </summary>
			public static Cursor Rotate
			{
				get
				{
					if (Cursors.cursorRotate == null)
					{
						Cursors.cursorRotate = GetCursor(@"Rotate.cur");
					}
					return Cursors.cursorRotate;
				}
			}

			/// <summary>
			/// Returns the InsertVertex cursor.
			/// </summary>
			public static Cursor InsertVertex
			{
				get
				{
					if (Cursors.cursorInsertVertex == null)
					{
						Cursors.cursorInsertVertex = GetCursor(@"InsertVertex.cur");
					}
					return Cursors.cursorInsertVertex;
				}
			}

			/// <summary>
			/// Returns the EditVertex cursor.
			/// </summary>
			public static Cursor EditVertex
			{
				get
				{
					if (Cursors.cursorEditVertex == null)
					{
						Cursors.cursorEditVertex = GetCursor(@"EditVertex.cur");
					}
					return Cursors.cursorEditVertex;
				}
			}

			/// <summary>
			/// Returns the DeleteVertex cursor.
			/// </summary>
			public static Cursor DeleteVertex
			{
				get
				{
					if (Cursors.cursorDeleteVertex == null)
					{
						Cursors.cursorDeleteVertex = GetCursor(@"DeleteVertex.cur");
					}
					return Cursors.cursorDeleteVertex;
				}
			}
            /// <summary>
            /// Returns the Pencil Cursor.
            /// </summary>
            public static Cursor Pencil
            {
                get
                {
                    if (Cursors.cursorPencil==null)
                    {
                        Cursors.cursorPencil = GetCursor(@"Pencil.cur");
                    }
                    return Cursors.cursorPencil;
                }
            }
		}
        /// <summary>
		/// Provides access to fill textures.
		/// </summary>
		public class Textures
		{
			/// <summary>
			/// Returns the CheckerBoard texture.
			/// </summary>
			public static System.Drawing.Image CheckerBoard
			{
				get
				{
					return (System.Drawing.Image) Resources.textureMgr.GetObject("CheckerBoard");
				}
			}
		}
	}
}
