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

namespace Syncfusion.Layouting
{
	/// <summary>
	/// Summary description for  ILayoutSpacingsInfo.
	/// </summary>
	public interface ILayoutSpacingsInfo
	{
    #region Properties
    /// <summary>
    /// Gets the paddings.
    /// </summary>
    /// <value>The paddings.</value>
    Spacings Paddings { get; }
    /// <summary>
    /// Gets the margins.
    /// </summary>
    /// <value>The margins.</value>
    Spacings Margins { get; }
    #endregion
	}
}
