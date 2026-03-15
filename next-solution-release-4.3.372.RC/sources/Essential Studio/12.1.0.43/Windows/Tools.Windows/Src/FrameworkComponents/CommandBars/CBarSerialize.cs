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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.IO;

using Syncfusion.Windows.Forms.Tools.XPMenus;

namespace Syncfusion.Windows.Forms.Tools
{
	/// CommandBar serialization wrappers should implement the ICommandBarSerializer interface
	/// along with ISerializable.
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface ICommandBarSerializer
	{
		void GetCommandBarData(CommandBar cbar);
		void SetCommandBarData(CommandBar cbar);
	}

	[Serializable]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class CBCtrlrSerializationWrapper
	{
		public CommandBarDockBorder cbBorder;
		protected Hashtable htCBarWrapper = new Hashtable();

		private const string DEF_SPACER = "_";

		public CBCtrlrSerializationWrapper(CommandBarController cbc)
		{
			this.InitSerailizationData(cbc);
		}

		public void CorrectCommandBarsOffset( CommandBarController cbc )
		{
			int count = cbc.CommandBars.Count;

			if( cbc != null && count > 0  )
			{
				for( int i = 0 ; i < count ; i++ )
				{
					for( int j = i ; j < count ; j++ )
					{
						CommandBar cb1 = cbc.CommandBars[i];
						CommandBar cb2 = cbc.CommandBars[j];

						if( cb1.Visible && cb2.Visible
							&& cb1.nRowOffsetDir == cb2.nRowOffsetDir
							&& cb1.nRCIndex == cb2.nRCIndex
							&& cb1.DockState == cb2.DockState 
							&& cb1.DockState != CommandBarDockState.Float )
						{
							if( cb1.DockState == CommandBarDockState.Top 
								|| cb1.DockState == CommandBarDockState.Bottom )
							{
								cb1.nRowOffsetDir = cb1.VLeft;
								cb2.nRowOffsetDir = cb2.VLeft;
							}
							else
							{
								cb1.nRowOffsetDir = cb1.VLocation.Y;
								cb2.nRowOffsetDir = cb2.VLocation.Y;
							}
						}
					}
				}
			}
		}

		public void InitSerailizationData(CommandBarController cbc)
		{
			this.cbBorder = cbc.EnabledDockBorders;
			
			this.CorrectCommandBarsOffset( cbc );

            foreach (CommandBar cbar in cbc.CommandBars)
            {
                this.AddCommandBarData(cbar);
            }
		}

		/// <summary>
		/// Gets name of CommandBar for save serialization data.
		/// </summary>
		private string GetCommandBarName( CommandBar cbar )
		{
			string commandBarName = String.Empty;

			if( cbar != null )
			{
				CommandBarExt cbarExt = cbar as CommandBarExt;

				commandBarName = ( cbarExt == null || cbarExt.MdiChildrenFormName == String.Empty ) ? 
					cbar.Name : cbar.Name + DEF_SPACER + cbarExt.MdiChildrenFormName;
			}

			return commandBarName;
		}

		public void AddCommandBarData( CommandBar cbar )
		{
            string commandBarName = GetCommandBarName( cbar );
            AddCommandBarData( cbar, commandBarName );
		}

        public void AddCommandBarData( CommandBar cbar, string commandBarName )
        {
            if( cbar.Visible )
            {
                if( commandBarName == String.Empty )
                {
                    throw new ApplicationException( "All CommandBars must have an unique name property for proper persistence." );
                }

                if (cbar.Controller != null)
                {
                    ICommandBarSerializer ibarserializer = Activator.CreateInstance(cbar.Controller.CommandBarSerializer) as ICommandBarSerializer;

                    if (ibarserializer == null)
                    {
                        throw new ApplicationException("Invalid CommandBar Serializer.");
                    }

                    CommandBarExt cb = cbar as CommandBarExt;
                    if (cb != null)
                    {
                        MainFrameBarManager manager = cb.Bar.Manager as MainFrameBarManager;
                        if (manager != null && manager.customAddedBarsVsNames.Contains(cb.Bar.BarName) &&
                            !manager.AutoSaveCustomData && !manager.ForceSaveLoadCustomData)
                        {
                            return;
                        }
                    }

                    ibarserializer.GetCommandBarData(cbar);
                    this.htCBarWrapper[commandBarName] = ibarserializer;
                }
            }
        }

        internal void RemoveCommandBarData(CommandBar cbar)
        {
            string commandBarName = GetCommandBarName(cbar);
            RemoveCommandBarData(commandBarName);
        }

        internal void RemoveCommandBarData(string commandBarName)
        {
            if (commandBarName == String.Empty)
            {
                throw new ApplicationException("All CommandBars must have an unique name property for proper persistence.");
            }

            if (this.htCBarWrapper.Contains(commandBarName))
                this.htCBarWrapper.Remove(commandBarName);
        }

		public void ReadDeserializedData(CommandBarController cbc)
		{
			cbc.EnabledDockBorders = this.cbBorder;
			foreach(CommandBar cbar in cbc.CommandBars)
				this.ReadDeserializedData(cbar);
		}

		public void ReadDeserializedData( CommandBar cbar )
		{
            string commandBarName = GetCommandBarName( cbar );
            ReadDeserializedData( cbar, commandBarName );
		}

        public void ReadDeserializedData( CommandBar cbar, string commandBarName )
		{
			if( cbar.Name == String.Empty )
			{
				throw new ApplicationException( "All CommandBars must have an unique name property for proper persistence." );
			}

            CommandBarExt cb = cbar as CommandBarExt;
            if (cb != null)
            {
                MainFrameBarManager manager = cb.Bar.Manager as MainFrameBarManager;
                if (manager != null && !manager.AutoLoadCustomData && !manager.ForceSaveLoadCustomData && manager.customAddedBarsVsNames.Contains(cb.Bar.BarName))
                {
                    return;
                }
            }

			if( this.htCBarWrapper.Contains( commandBarName ) )
			{
				((this.htCBarWrapper[commandBarName]) as ICommandBarSerializer).SetCommandBarData( cbar );
			}
		}

        /// <summary>
        /// Indicating whether state of the CommandBar loaded from isolated storage.
        /// </summary>
        public bool IsInitialized( string commandBarName )
		{
            return htCBarWrapper.ContainsKey( commandBarName );
		}

        /// <summary>
        /// Indicating whether state of the CommandBar loaded from isolated storage.
        /// </summary>
        public bool IsInitialized( CommandBar cbar )
		{
			bool bInitialized = false;

			if( cbar != null )
			{
				string commandBarName = GetCommandBarName( cbar );
				bInitialized = htCBarWrapper.ContainsKey( commandBarName );
			}

			return bInitialized;
		}
	}

	[Serializable]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class CBarSerializationWrapper : ISerializable, ICommandBarSerializer
	{
		// ID
		public String strName;

		// Layout attributes
		public int nMaxLength;
		public int nMinLength;
		public int nCommandBarHt;
        public int nRCCountDrag;

		// State information
		public CommandBarDockState cbarDockState;
		public int nRowOffsetDir;
		public int nRCIndex;
		public bool bVisible;
		public Point ptFloat;
        public Size szFloat;

		public CBarSerializationWrapper()
		{
		}

		// ICommandBarSerializer implementation
		public void GetCommandBarData(CommandBar cbar)
		{
			this.nMaxLength = cbar.nMaxLength;
			this.nMinLength = cbar.nMinLength;
			this.nCommandBarHt = cbar.nCommandBarHt;
            this.nRCCountDrag = cbar.nRCCountDrag;

			this.cbarDockState = cbar.cbarDockState;
			this.nRowOffsetDir = cbar.nRowOffsetDir;
			this.nRCIndex = cbar.nRCIndex;
			this.bVisible = cbar.bVisible;
			this.ptFloat = cbar.FloatBounds.Location;
            this.szFloat = cbar.FloatBounds.Size;
		}

		public void SetCommandBarData(CommandBar cbar)
		{
			cbar.nMaxLength = this.nMaxLength;
			cbar.nMinLength = this.nMinLength;
			cbar.nCommandBarHt = this.nCommandBarHt;
            cbar.nRCCountDrag = this.nRCCountDrag;

			cbar.cbarDockState = this.cbarDockState;
			cbar.nRowOffsetDir = this.nRowOffsetDir;
			cbar.nRowOffsetInDir = cbar.nRowOffsetDir;
			cbar.nRCIndex = this.nRCIndex;
			cbar.bVisible = this.bVisible;
			cbar.rcFloat = new Rectangle(this.ptFloat, this.szFloat);
		}

		// Private constructor called during the deserialization process
		protected CBarSerializationWrapper(SerializationInfo info, StreamingContext context)
		{
			this.nMaxLength = info.GetInt32("MaxLength");
			this.nMinLength = info.GetInt32("MinLength");
			this.nCommandBarHt = info.GetInt32("MinHeight");

            try
            {
                //Need to skip RCCoundDrag - if it isn't included into serialization info.
                this.nRCCountDrag = info.GetInt32("RCCountDrag");
                this.szFloat = (Size)info.GetValue("FloatSize", typeof(Size));
            }
            catch (SerializationException)
            {
                this.nRCCountDrag = 0;
                this.szFloat = Size.Empty;
            }

			this.cbarDockState = (CommandBarDockState)info.GetValue("CommandBarDockState", typeof(CommandBarDockState));
			this.nRowOffsetDir = info.GetInt32("RowOffsetDir");
			this.nRCIndex = info.GetInt32("RCIndex");
			this.bVisible = info.GetBoolean("CommandBarVisibility");
			this.ptFloat = (Point)info.GetValue("FloatLocation", typeof(Point));
		}

		// ISerializable implementation
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("MaxLength", this.nMaxLength);
			info.AddValue("MinLength", this.nMinLength);
			info.AddValue("MinHeight", this.nCommandBarHt);
            info.AddValue("RCCountDrag", this.nRCCountDrag);

			info.AddValue("CommandBarDockState", this.cbarDockState);
			info.AddValue("RowOffsetDir", this.nRowOffsetDir);
			info.AddValue("RCIndex", this.nRCIndex);
			info.AddValue("CommandBarVisibility", this.bVisible);
			info.AddValue("FloatLocation", this.ptFloat);
            info.AddValue("FloatSize", this.szFloat);
		}
	}
}