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

#region File using directives
using System;
#endregion

namespace Syncfusion.DLS
{
	/// <summary>
	/// Summary description for ITextBoxCollection.
	/// </summary>
	public interface ITextBoxCollection
	{
    /// <summary>
    /// Get textbox item from textbox collection
    /// </summary>
    ITextBox this[ int index ] { get; }
    /// <summary>
    /// Add textbox to the collection of textboxes
    /// </summary>
    /// <param name="textBox"></param>
    /// <returns></returns>
    int Add( ITextBox textBox );
	}
}
