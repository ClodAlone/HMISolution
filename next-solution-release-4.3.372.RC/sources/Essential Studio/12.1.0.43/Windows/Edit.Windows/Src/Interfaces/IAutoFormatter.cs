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

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
	/// <summary>
	/// Interface for custom autoformatters.
	/// </summary>
	public interface IAutoFormatter
	{
		#region Interface Methods
		/// <summary>
		/// Formats given list of lexem wrappers.
		/// </summary>
		/// <param name="lexems">List of ILexemWrapper instances.</param>
		/// <returns>String with formatted text.</returns>
    string Format( IList lexems );
		#endregion
	}
}