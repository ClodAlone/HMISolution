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
using System.IO;

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
  /// A collection of cell comments. Each comment is represented by a
  /// Comment object.
  /// </summary>
  public interface IPictures : IParentApplication, IEnumerable
  {
    #region Interface properties
    /// <summary>
    /// Returns the number of objects in the collection. Read-only Long.
    /// </summary>
    int Count { get; }
    /// <summary>
    /// Returns a single object from a collection.
    /// </summary>
    IPictureShape this[ int Index ]{ get; }
    /// <summary>
    /// Gets single item from the collection.
    /// </summary>
    /// <param name="name">Name of the item to get.</param>
    /// <returns>Single item from the collection.</returns>
    IPictureShape this[ string name ] { get; }
    #endregion

    #region Interface methods
#if !(WINRT )
    /// <summary>
    /// Adds picture to the collection.
    /// </summary>
    /// <param name="image">Picture to add.</param>
    /// <param name="pictureName">Picture name.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( Image image, string pictureName );
    /// <summary>
    /// Adds picture to the collection.
    /// </summary>
    /// <param name="image">Picture to add.</param>
    /// <param name="pictureName">Picture name.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( Image image, string pictureName, ExcelImageFormat imageFormat );

    /// <summary>
    /// Adds picture from the specified file.
    /// </summary>
    /// <param name="strFileName">Picture file name.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( string strFileName );
    /// <summary>
    /// Adds picture from the specified file.
    /// </summary>
    /// <param name="strFileName">Picture file name.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( string strFileName, ExcelImageFormat imageFormat );
#endif
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="image">Image.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn, Image image );
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="image">Image to add.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn, Image image,
      ExcelImageFormat imageFormat );

    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="stream">Stream with the picture.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn, Stream stream );
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="stream">Stream with the picture.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn, Stream stream,
      ExcelImageFormat imageFormat );
#if !(WINRT )
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="fileName">Name of the shape.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn, string fileName );
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="fileName">Name of the shape.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn, string fileName,
      ExcelImageFormat imageFormat );
#endif
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="bottomRow">Bottom row.</param>
    /// <param name="rightColumn">Right column.</param>
    /// <param name="image">Image.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn,
      int bottomRow, int rightColumn, Image image );
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="bottomRow">Bottom row.</param>
    /// <param name="rightColumn">Right column.</param>
    /// <param name="image">Image to add.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn,
      int bottomRow, int rightColumn, Image image, ExcelImageFormat imageFormat );

    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="bottomRow">Bottom row.</param>
    /// <param name="rightColumn">Right column.</param>
    /// <param name="stream">Stream with the picture.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn,
      int bottomRow, int rightColumn, Stream stream );
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="bottomRow">Bottom row.</param>
    /// <param name="rightColumn">Right column.</param>
    /// <param name="stream">Stream with the picture.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn,
      int bottomRow, int rightColumn, Stream stream, ExcelImageFormat imageFormat );
#if !(WINRT )
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="bottomRow">Bottom row.</param>
    /// <param name="rightColumn">Right column.</param>
    /// <param name="fileName">Name of the shape.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn,
      int bottomRow, int rightColumn, string fileName );
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="bottomRow">Bottom row.</param>
    /// <param name="rightColumn">Right column.</param>
    /// <param name="fileName">Name of the shape.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn,
      int bottomRow, int rightColumn, string fileName, ExcelImageFormat imageFormat );
#endif
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="image">Image.</param>
    /// <param name="scaleWidth">Width scale in percents.</param>
    /// <param name="scaleHeight">Height scale in percents.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn, Image image,
      int scaleWidth, int scaleHeight );
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="image">Image.</param>
    /// <param name="scaleWidth">Width scale in percents.</param>
    /// <param name="scaleHeight">Height scale in percents.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn, Image image,
      int scaleWidth, int scaleHeight, ExcelImageFormat imageFormat );

    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="stream">Stream with the picture.</param>
    /// <param name="scaleWidth">Width scale in percents.</param>
    /// <param name="scaleHeight">Height scale in percents.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn, Stream stream,
      int scaleWidth, int scaleHeight );
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="stream">Stream with the picture.</param>
    /// <param name="scaleWidth">Width scale in percents.</param>
    /// <param name="scaleHeight">Height scale in percents.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn, Stream stream,
      int scaleWidth, int scaleHeight, ExcelImageFormat imageFormat );
#if !(WINRT )
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="fileName">Name of the shape.</param>
    /// <param name="scaleWidth">Width scale in percents.</param>
    /// <param name="scaleHeight">Height scale in percents.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn, string fileName,
      int scaleWidth, int scaleHeight );
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="fileName">Name of the shape.</param>
    /// <param name="scaleWidth">Width scale in percents.</param>
    /// <param name="scaleHeight">Height scale in percents.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    IPictureShape AddPicture( int topRow, int leftColumn, string fileName,
      int scaleWidth, int scaleHeight, ExcelImageFormat imageFormat );
#endif
    #endregion
  }
}
