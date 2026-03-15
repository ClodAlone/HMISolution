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

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Interface for toolstrip items able to reflect ToolStripEx or ToolStripItem.
	/// Implemented for items that can be shown in quick panel only.
	/// </summary>
	public interface IQuickItem
	{
		#region Methods
		/// <summary>
		/// 
		/// </summary>
		void Reset();
		/// <summary>
		/// Indicates whether item reflects given component.
		/// </summary>
		/// <param name="c">Component to check.</param>
		/// <returns>True if item reflects given component; otherwise false.</returns>
		bool Reflects( IComponent c );
		#endregion

		#region Properties
		/// <summary>
		/// Gets reflected component.
		/// </summary>
		Component ReflectedComponent { get; }
		#endregion
	}
}

#endif
