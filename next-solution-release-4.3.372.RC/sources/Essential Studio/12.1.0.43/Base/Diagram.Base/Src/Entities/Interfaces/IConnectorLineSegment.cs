#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Interface that handles line segment actions.
	/// </summary>
	public interface IConnectorLineSegment
	{
		/// <summary>
		/// Move segment to offset.
		/// </summary>
		/// <param name="szOffset">Move offset.</param>
		void Move( SizeF szOffset );
		/// <summary>
		/// Synchronization segment to handle changes.
		/// </summary>
		/// <param name="handle">The handle synchronize to.</param>
		void SyncSegmentHandle( IHandle handle );
	}
}
