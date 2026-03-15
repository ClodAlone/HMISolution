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

namespace Syncfusion.Windows.Forms
{
     /// <summary>
    ///   <see cref="SplitterLayout"/> holds information about the current vertical and horizontal split positions.
    /// </summary>
    public class SplitterLayout
    {
        internal int _hSplitPos = 0;
        internal int _vSplitPos = 0;

		/// <summary>
		/// Gets / sets the horizontal splitter position in pixels.
		/// </summary>
		public int HSplitPos
		{
			get
			{
				return _hSplitPos;
			}
			set
			{
				_hSplitPos = value;
			}
		}

		/// <summary>
		/// Gets / sets the vertical splitter position in pixels.
		/// </summary>
		public int VSplitPos
		{
			get
			{
				return _vSplitPos;
			}
			set
			{
				_vSplitPos = value;
			}
		}
	}
}
