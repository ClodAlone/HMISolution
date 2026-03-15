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

using System.Drawing;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
	/// <summary>
	/// Base handle interface that contain all based transformation properties and methods.
	/// </summary>
	public interface IHandle
	{
		/// <summary>
		/// Gets the handle ID.
		/// </summary>
		/// <value>The handle ID.</value>
		int ID{ get; }
		/// <summary>
		/// Gets the handle location.
		/// </summary>
		/// <value>The handle location.</value>
		PointF Location{ get; }
		/// <summary>
		/// Gets or sets a value indicating whether allow move by X axis.
		/// </summary>
		/// <value><c>true</c> if allow move by X axis; otherwise, <c>false</c>.</value>
		bool AllowMoveX{ get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether allow move by Y axis.
		/// </summary>
		/// <value><c>true</c> if allow move by Y axis; otherwise, <c>false</c>.</value>
		bool AllowMoveY{ get; set; }
		/// <summary>
		/// Move the specified offset.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <param name="offsetUnits">The offset units.</param>
		void Move( SizeF offset, MeasureUnits offsetUnits );
        /// <summary>
        /// Gets or sets a value indicating whether the handle is updating
        /// </summary>
        /// <value><c>true</c> if handle is updating; otherwise, <c>false</c>.</value>
        bool InUpdate { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the handle is moving
        /// </summary>
        /// <value><c>true</c> if handle is moving; otherwise, <c>false</c>.</value>
        bool IsMoving { get; set; }
	}
}
