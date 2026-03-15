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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using Syncfusion.Collections;

namespace Syncfusion.Windows.Forms.Tools
{
	#region ToolStripItemAdvInfo
	/// <summary>
	/// Info about ToolStripItem in the RibbonControlAdvHeader.
	/// </summary>
	[TypeConverter(typeof(Design.ToolStripItemAdvInfoTypeConverter))]
	public class ToolStripItemAdvInfo
	{
		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of ToolStripItemAdvInfo.
		/// </summary>
		/// <param name="item">Name of the underlying item.</param>
		public ToolStripItemAdvInfo( ToolStripItem item)
		{
			Item = item;
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		internal virtual int Order
		{
			get { return 0; }
		}
		/// <summary>
		/// 
		/// </summary>
		internal virtual bool RaiseAddedEvent
		{
			get { return true; }
		}
		#endregion

		#region Fields
		/// <summary>
		/// Reference to the underlying item.
		/// </summary>
		public ToolStripItem Item;
		#endregion
	}
	#endregion

	#region DropDownButtonAdvInfo
	/// <summary>
	/// 
	/// </summary>
	public class DropDownButtonAdvInfo
		: ToolStripItemAdvInfo
	{
		public DropDownButtonAdvInfo( ToolStripItem item )
			: base( item ){}
		internal override bool RaiseAddedEvent
		{
			get{return false;}
		}
		internal override int Order
		{
			get{return 1;}
		}
	}
	#endregion
}
#endif