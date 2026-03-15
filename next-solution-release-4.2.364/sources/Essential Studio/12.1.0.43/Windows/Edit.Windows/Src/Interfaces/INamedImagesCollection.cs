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
using System.Drawing;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
	/// <summary>
	/// Provides read-write access to the collection of the named images.
	/// </summary>
	public interface INamedImagesCollection
		: ICollection
	{
		/// <summary>
		/// Gets image by it's name.
		/// </summary>
		INamedImage this[ string index ]{ get; }
		/// <summary>
		/// Gets image by it's index.
		/// </summary>
		INamedImage this[ int index ]{ get; }
		/// <summary>
		/// Creates and adds new named image to the collection.
		/// </summary>
		/// <param name="name">Name of the image to be added.</param>
		/// <param name="image">Image to be added.</param>
		/// <returns>INamedImage object.</returns>
		INamedImage AddImage( string name, Image image );
		/// <summary>
		/// Creates and adds new named image to the collection.
		/// </summary>
		/// <param name="name">Name of the image to be added.</param>
		/// <param name="image">Image to be added.</param>
		/// <param name="transparent">Transparent color of the image.</param>
		/// <returns>INamedImage object.</returns>
		INamedImage AddImage( string name, Image image, Color transparent );
	}
}
