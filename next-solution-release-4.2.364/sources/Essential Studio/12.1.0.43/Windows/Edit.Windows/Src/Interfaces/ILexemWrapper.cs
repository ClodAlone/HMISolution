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

using Syncfusion.Windows.Forms.Edit.Implementation.Parser;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
	/// <summary>
	/// Interface for lexem wrapper. Used in autoformatting.
	/// </summary>
	public interface ILexemWrapper
	{
		#region Interface Properties
		/// <summary>
		/// Gets text of the lexem.
		/// </summary>
		string Text{ get; }
		/// <summary>
		/// Gets configuration of the lexem.
		/// </summary>
		IConfigLexem Config{ get; }
		/// <summary>
		/// Gets configuration stack of the lexem.
		/// </summary>
		ConfigStack Stack{ get; }
		#endregion
	}
}
