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

using System.Text.RegularExpressions;

using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Implementation.IO
{
	#region *** FindResult
	/// <summary>
	/// Result of the find operations.
	/// </summary>
	public struct FindResult
	{
		/// <summary>
		/// Start point of the result.
		/// </summary>
		public IParsePoint StartPoint;
		/// <summary>
		/// End point of the result.
		/// </summary>
		public IParsePoint EndPoint;
		/// <summary>
		/// Result of the RegExp search.
		/// </summary>
		public Match Result;
		/// <summary>
		/// Empty structure.
		/// </summary>
		public static FindResult Empty
		{
			get
			{
				return new FindResult();
			}
		}
		/// <summary>
		/// Gets value indicating whether find result is empty.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return ( StartPoint == null && EndPoint == null && Result == null );
			}
		}
	}
	#endregion

	#region *** WrapperUndoItem
	/// <summary>
	/// Single undo item.
	/// </summary>
	public struct WrapperUndoItem
	{
		/// <summary>
		/// Change context, that was applied to the stream.
		/// </summary>
		public ChangeContext ChangeContext;
		/// <summary>
		/// Any additional data.
		/// </summary>
		public object AdditionalData;
		/// <summary>
		/// Offset, in bytes.
		/// </summary>
		public long BytesOffset;
		/// <summary>
		/// Empty <see cref="WrapperUndoItem"/>.
		/// </summary>
		public static WrapperUndoItem Empty
		{
			get
			{
				return new WrapperUndoItem();
			}
		}
	}
	#endregion
}
