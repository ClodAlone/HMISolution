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

#region file using directives
using System;
using System.Collections;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
using System.Drawing;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
using System.Drawing;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#endregion

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Represents shapes collection in the workbook.
  /// </summary>
  public interface IShapes
    : IParentApplication
    , IEnumerable
  {
    #region Interface methods
    /// <summary>
    /// Adds new image to the collection.
    /// </summary>
    /// <param name="image">Image to add.</param>
    /// <param name="pictureName">File name with the image.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Newly created picture shape.</returns>
    IPictureShape AddPicture( Image image, string pictureName, ExcelImageFormat imageFormat );
#if !(WINRT )
    /// <summary>
    /// Adds new image to the collection.
    /// </summary>
    /// <param name="fileName">File name with the image.</param>
    /// <returns>Newly created picture shape.</returns>
    IPictureShape AddPicture( string fileName );
#endif
    /// <summary>
    /// Adds new Comment shape to the collection.
    /// </summary>
    /// <param name="commentText">Text of the comment.</param>
    /// <param name="bIsParseOptions">Indicates is parse comment fill line options.</param>
    /// <returns>Newly added shape.</returns>
    ICommentShape  AddComment( string commentText, bool bIsParseOptions );
    /// <summary>
    /// Adds new Comment shape to the collection.
    /// </summary>
    /// <param name="commentText">Text of the comment.</param>
    /// <returns>Newly added shape.</returns>
    ICommentShape AddComment( string commentText );
    /// <summary>
    /// Adds new Comment shape with empty comment text to the collection.
    /// </summary>
    /// <returns>Newly added shape.</returns>
    ICommentShape AddComment();
    /// <summary>
    /// Adds new chart shape to the collection.
    /// </summary>
    /// <returns>Newly added shape.</returns>
    IChartShape   AddChart();
    /// <summary>
    /// Adds shape copy to the collection.
    /// </summary>
    /// <param name="sourceShape">Shape to copy.</param>
    /// <returns>Added shape.</returns>
    IShape AddCopy( IShape sourceShape );
    /// <summary>
    /// Adds shape copy to shapes collection.
    /// </summary>
    /// <param name="sourceShape">Shape to copy.</param>
    /// <param name="hashNewNames">Dictionary with new names of worksheets.</param>
    /// <param name="arrFontIndexes">List with new font indexes.</param>
    /// <returns>Added shape.</returns>
    IShape AddCopy( IShape sourceShape
      , Dictionary<string, string> hashNewNames, List<int> arrFontIndexes );
    /// <summary>
    /// Adds new text box to the collection.
    /// </summary>
    /// <returns>Newly created textbox.</returns>
    ITextBoxShapeEx AddTextBox();
    /// <summary>
    /// Adds new check box to the collection.
    /// </summary>
    /// <returns>Newly created checkbox.</returns>
    ICheckBoxShape AddCheckBox();
    /// <summary>
    /// Adds new ootion button to the collection.
    /// </summary>
    /// <returns>Newly created checkbox.</returns>
    IOptionButtonShape AddOptionButton();
    /// <summary>
    /// Adds new check box to the collection.
    /// </summary>
    /// <returns>Newly created checkbox.</returns>
    IComboBoxShape AddComboBox();

    IShape AddAutoShapes(AutoShapeType autoShapeType, int topRow, int leftColumn,int height, int width);
    #endregion

    #region Interface properties
    /// <summary>
    /// Returns the number of objects in the collection. Read-only Long.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns single shape from the collection by its index.
    /// </summary>
    IShape this[ int index ] { get; }
    /// <summary>
    /// Returns single shape from the collection by its name or null when cannot find. Read-only.
    /// </summary>
    IShape this[ string strShapeName ] { get; }
    #endregion
  }
}