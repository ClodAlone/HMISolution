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
using System.Drawing;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
	/// <summary>
	/// Object that represents named image.
	/// </summary>
	public interface INamedImage
	{
    /// <summary>
    /// Gets name of the image.
    /// </summary>
    string Name{ get; }
    /// <summary>
    /// Gets image.
    /// </summary>
    Image Image{ get; }
    /// <summary>
    /// Gets transparent color of the image.
    /// </summary>
    Color TransparentColor{ get; }
    /// <summary>
    /// Deletes image from collection.
    /// </summary>
    void Delete();
  }
}
