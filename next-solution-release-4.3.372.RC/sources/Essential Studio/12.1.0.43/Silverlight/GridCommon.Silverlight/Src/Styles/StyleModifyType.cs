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

#if !WinRT
namespace Syncfusion.Windows.Styles
#else
namespace Syncfusion.WinRT.Styles
#endif
{
	/// <summary>
	/// StyleModifyType defines style operations for <see cref="StyleInfoBase.ModifyStyle"/>.
	/// </summary>
	public enum StyleModifyType
	{
		/// <summary>
		/// Copies all initialized properties.
		/// </summary>
		Override,
		/// <summary>
		/// Copies only properties that have not been initialized in the target style object.
		/// </summary>
		ApplyNew,
		/// <summary>
		/// Copies all properties and resets properties in the target style.
		/// </summary>
		Copy,
		/// <summary>
		/// Resets properties in the target style that have been marked as initialized in the source style.
		/// </summary>
		Exclude,
		/// <summary>
		/// Clears out all properties.
		/// </summary>
		Remove,
		/// <summary>
		/// Copies and resets all properties in the target style when the property has been marked as changed in the source style.
		/// </summary>
		Changes
	}
}
