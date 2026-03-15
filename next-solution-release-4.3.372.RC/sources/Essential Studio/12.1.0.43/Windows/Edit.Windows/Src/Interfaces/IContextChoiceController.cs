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
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
	/// <summary>
	/// Controller for the context choice.
	/// </summary>
	public interface IContextChoiceController
	{
    /// <summary>
    /// Gets or sets value indicating whether autocomplete 
    /// tehnique should be used with current context choice.
    /// </summary>
    bool UseAutocomplete{ get; set; }
		/// <summary>
		/// Gets or sets value that specifies whether autocomplete string should be extended.
		/// </summary>
		bool ExtendItemsFilteringString { get; set; }
		/// <summary>
    /// Gets or sets currently selected item.
    /// </summary>
    IContextChoiceItem SelectedItem{ get; set; }
    /// <summary>
    /// Gets collection of the context choice items.
    /// </summary>
    ContextChoiceItemCollection Items{ get; }
    /// <summary>
    /// Gets collection of the INamedImage items.
    /// </summary>
    INamedImagesCollection Images{ get; }
    /// <summary>
    /// Gets value indicating whether context choice window associated with current controller is visible.
    /// </summary>
    bool IsVisible{ get; }
		/// <summary>
		/// Gets or sets size of the context choice form.
		/// </summary>
		Size FormSize{ get; set; }
		/// <summary>
		/// Gets or sets dropping lexem.
		/// </summary>
		IRenderedLexem Dropper{ get; set; }
		/// <summary>
		/// Gets or sets lexem situated before dropper.
		/// </summary>
		IRenderedLexem LexemBeforeDropper{ get; set; }
		/// <summary>
    /// Adds image to the internal image list.
    /// </summary>
    /// <param name="name">Name of the image. Must be unique.</param>
    /// <param name="img">The image to be added.</param>
    /// <param name="transparent">Transparent color.</param>
    /// <returns>Index of the added image.</returns>
    INamedImage AddImage( string name, Image img, Color transparent );
    /// <summary>
    /// Adds image to the internal image list.
    /// </summary>
    /// <param name="name">Name of the image. Must be unique.</param>
    /// <param name="img">The image to be added.</param>
    /// <returns>Index of the added image.</returns>
    INamedImage AddImage( string name, Image img );
    /// <summary>
    /// Adds image to the internal image list.
    /// </summary>
    /// <param name="img">The image to be added.</param>
    /// <param name="transparent">Transparent color.</param>
    /// <returns>Index of the added image.</returns>
    INamedImage AddImage( Image img, Color transparent );
    /// <summary>
    /// Adds image to the internal image list.
    /// </summary>
    /// <param name="img">The image to be added.</param>
    /// <returns>Index of the added image.</returns>
    INamedImage AddImage( Image img );
  }
}
