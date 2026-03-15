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
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Shapes;

#if ( WINRT)
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using System.Drawing;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using System.Drawing;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  ///
  /// </summary>
  public class PicturesCollection
    : CollectionBaseEx<IPictureShape>
    , IPictures
  {
    #region Class constants
    /// <summary>
    /// Default prefix for picture name.
    /// </summary>
    private string DEF_PICTURE_NAME = "Picture";

    private string[] m_indexedpixel_notsupport = { "Format1bppIndexed", "Format4bppIndexed", "Format8bppIndexed", "b96b3cac-0728-11d3-9d7b-0000f81ef32e", "b96b3cad-0728-11d3-9d7b-0000f81ef32e" };
    #endregion

    #region Class members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetBaseImpl m_sheet;
    #endregion

    #region Properties
    /// <summary>
    /// Gets single item from the collection.
    /// </summary>
    /// <param name="name">Name of the item to get.</param>
    /// <returns>Single item from the collection.</returns>
    public IPictureShape this[ string name ]
    {
      get
      {
        IPictureShape result = null;

        for( int i = 0, len = Count; i < len; i++ )
        {
          IPictureShape currentShape = this[ i ];

          if( currentShape.Name == name )
          {
            result = currentShape;
            break;
          }
        }

        return result;
      }
    }
    #endregion

    #region Interface methods
    /// <summary>
    /// Adds picture to the collection.
    /// </summary>
    /// <param name="image">Picture to add.</param>
    /// <param name="pictureName">Picture name.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( Image image, string pictureName )
    {
      return AddPicture( image, pictureName, ExcelImageFormat.Original ) as IPictureShape;
    }
    /// <summary>
    /// Adds picture to the collection.
    /// </summary>
    /// <param name="image">Picture to add.</param>
    /// <param name="pictureName">Picture name.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( Image image, string pictureName, ExcelImageFormat imageFormat )
    {
      IShapes shapes = m_sheet.Shapes;
      return shapes.AddPicture( image, pictureName, imageFormat ) as IPictureShape;
    }
#if !(WINRT )
    /// <summary>
    /// Adds picture from the specified file.
    /// </summary>
    /// <param name="strFileName">Picture file name.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( string strFileName )
    {
      return AddPicture( strFileName, ExcelImageFormat.Original );
    }

    /// <summary>
    /// Adds picture from the specified file.
    /// </summary>
    /// <param name="strFileName">Picture file name.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( string strFileName, ExcelImageFormat imageFormat )
    {
      IShapes shapes = m_sheet.Shapes;
      IPictureShape picture = shapes.AddPicture( strFileName ) as IPictureShape;
      
      return picture;
    }
#endif
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="image">Image.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( int topRow, int leftColumn, Image image )
    {
      return AddPicture( topRow, leftColumn, image, ExcelImageFormat.Original );
    }
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="image">Image to add.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( int topRow, int leftColumn, Image image,
      ExcelImageFormat imageFormat )
    {
      if( image == null )
        throw new ArgumentNullException( "image" );

      BitmapShapeImpl result = ( BitmapShapeImpl )AddPicture( image, GeneratePictureName(), imageFormat );
      result.LeftColumn = leftColumn;
      result.TopRow = topRow;

      result.EvaluateTopLeftPosition();

      return result;
    }
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="stream">Stream with the picture.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( int topRow, int leftColumn, Stream stream )
    {
      return AddPicture( topRow, leftColumn, stream, ExcelImageFormat.Original );
    }
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="stream">Stream with the picture.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( int topRow, int leftColumn, Stream stream,
      ExcelImageFormat imageFormat )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      Image image = ApplicationImpl.CreateImage( stream );
      
      return AddPicture( topRow, leftColumn, image, imageFormat );
    }
#if !(WINRT )
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="fileName">Name of the shape.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( int topRow, int leftColumn, string fileName )
    {
      return AddPicture( topRow, leftColumn, fileName, ExcelImageFormat.Original );
    }
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="fileName">Name of the shape.</param>
    /// <param name="imageFormat">Image format to use for picture storing.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( int topRow, int leftColumn, string fileName,
      ExcelImageFormat imageFormat )
    {
      if( fileName == null )
        throw new ArgumentNullException( "pictureName" );

      if( fileName.Length == 0 )
        throw new ArgumentException( "pictureName can't be empty" );

      //Image image = Image.FromFile( fileName );
#if WP
      Stream stream = File.Open(fileName, FileMode.Open);
#else
      MemoryStream stream = new MemoryStream(File.ReadAllBytes(fileName));
#endif
      Image image = ApplicationImpl.CreateImage( stream );
      
      IPictureShape result = AddPicture( topRow, leftColumn, image, imageFormat );
      result.Name = Path.GetFileNameWithoutExtension( fileName );

      return result;
    }
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
    public IPictureShape AddPicture( int topRow, int leftColumn,
      int bottomRow, int rightColumn, Image image )
    {
      if( image == null )
        throw new ArgumentNullException( "image" );

      return AddPicture( topRow, leftColumn, bottomRow, rightColumn, image, ExcelImageFormat.Original );
    }
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
    public IPictureShape AddPicture( int topRow, int leftColumn,
      int bottomRow, int rightColumn, Image image, ExcelImageFormat imageFormat )
    {
      if( image == null )
        throw new ArgumentNullException( "image" );

      BitmapShapeImpl result = ( BitmapShapeImpl )AddPicture( topRow, leftColumn, image, imageFormat );

      result.RightColumn = rightColumn;
      result.BottomRow = bottomRow;
      result.UpdateHeight();
      result.UpdateWidth();
      result.ClearShapeOffset(true);

      return result;
    }
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="bottomRow">Bottom row.</param>
    /// <param name="rightColumn">Right column.</param>
    /// <param name="stream">Stream with the picture.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( int topRow, int leftColumn,
      int bottomRow, int rightColumn, Stream stream )
    {
      return AddPicture( topRow, leftColumn, bottomRow, rightColumn,
        stream, ExcelImageFormat.Original );
    }
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
    public IPictureShape AddPicture( int topRow, int leftColumn,
      int bottomRow, int rightColumn, Stream stream, ExcelImageFormat imageFormat )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      Image image = ApplicationImpl.CreateImage( stream );
      
      return AddPicture( topRow, leftColumn, bottomRow, rightColumn, image, imageFormat );
    }
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
    public IPictureShape AddPicture( int topRow, int leftColumn,
      int bottomRow, int rightColumn, string fileName )
    {
      return AddPicture( topRow, leftColumn, bottomRow, rightColumn,
        fileName, ExcelImageFormat.Original );
    }
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
    public IPictureShape AddPicture( int topRow, int leftColumn,
      int bottomRow, int rightColumn, string fileName, ExcelImageFormat imageFormat )
    {
      if( fileName == null )
        throw new ArgumentNullException( "pictureName" );

      if( fileName.Length == 0 )
        throw new ArgumentException( "pictureName can't be empty." );

      Image image = Image.FromFile( fileName );
      
      IPictureShape result = AddPicture( topRow, leftColumn, bottomRow, rightColumn, image );
      result.Name = Path.GetFileNameWithoutExtension( fileName );
#if !SILVERLIGHT && !WINRT && !WP
      int unsupport = Array.IndexOf(m_indexedpixel_notsupport, image.RawFormat.Guid.ToString());
      if (unsupport == -1)
      {
          image.Dispose();
      }
#endif
      return result;
    }
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
    public IPictureShape AddPicture( int topRow, int leftColumn, Image image,
      int scaleWidth, int scaleHeight )
    {
      return AddPicture( topRow, leftColumn, image, scaleWidth, scaleHeight,
        ExcelImageFormat.Original );
    }
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
    public IPictureShape AddPicture( int topRow, int leftColumn, Image image,
      int scaleWidth, int scaleHeight, ExcelImageFormat imageFormat )
    {
      if( image == null )
        throw new ArgumentNullException( "image" );

      IPictureShape result = AddPicture( topRow, leftColumn, image, imageFormat );
      result.Scale( scaleWidth, scaleHeight );

      return result;
    }
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="stream">Stream with the picture.</param>
    /// <param name="scaleWidth">Width scale in percents.</param>
    /// <param name="scaleHeight">Height scale in percents.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( int topRow, int leftColumn, Stream stream,
      int scaleWidth, int scaleHeight )
    {
      return AddPicture( topRow, leftColumn, stream, scaleWidth, scaleHeight,
        ExcelImageFormat.Original );
    }
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
    public IPictureShape AddPicture( int topRow, int leftColumn, Stream stream,
      int scaleWidth, int scaleHeight, ExcelImageFormat imageFormat )
    {
        Image image = ApplicationImpl.CreateImage(stream);

        IPictureShape result = AddPicture(topRow, leftColumn, image, imageFormat);
        result.Scale(scaleWidth, scaleHeight);
        return result;
    }
#if !(WINRT)
    /// <summary>
    /// Adds image to the collection.
    /// </summary>
    /// <param name="topRow">Top row of a new picture.</param>
    /// <param name="leftColumn">Left column.</param>
    /// <param name="fileName">Name of the shape.</param>
    /// <param name="scaleWidth">Width scale in percents.</param>
    /// <param name="scaleHeight">Height scale in percents.</param>
    /// <returns>Added picture.</returns>
    public IPictureShape AddPicture( int topRow, int leftColumn, string fileName,
      int scaleWidth, int scaleHeight )
    {
      return AddPicture( topRow, leftColumn, fileName, scaleWidth, scaleHeight, ExcelImageFormat.Original );
    }
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
    public IPictureShape AddPicture( int topRow, int leftColumn, string fileName,
      int scaleWidth, int scaleHeight, ExcelImageFormat imageFormat )
    {
      if( fileName == null )
        throw new ArgumentNullException( "pictureName" );

      if( fileName.Length == 0 )
        throw new ArgumentException( "pictureName can't be empty." );

      IPictureShape result = AddPicture( topRow, leftColumn, fileName, imageFormat );
      result.Scale( scaleWidth, scaleHeight );

      return result;
    }
#endif
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public PicturesCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    #endregion

    #region Class Methods
    /// <summary>
    /// Removes picture from this collection only.
    /// </summary>
    /// <param name="picture">Picture to remove.</param>
    internal void RemovePicture( IPictureShape picture )
    {
      InnerList.Remove( picture );
    }
    /// <summary>
    /// Adds picture to this collection only.
    /// Should be called from Shapes collection only.
    /// </summary>
    /// <param name="picture">Picture to add.</param>
    internal void AddPicture( IPictureShape picture )
    {
      InnerList.Add( picture );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// Can't find parent worksheet.
    /// </exception>
    private void SetParents()
    {
      m_sheet = FindParent( typeof( WorksheetBaseImpl ), true ) as WorksheetBaseImpl;

      if( m_sheet == null )
        throw new ArgumentNullException( "Can't find parent worksheet." );
    }
    /// <summary>
    /// 
    /// </summary>
    private string GeneratePictureName()
    {
      return DEF_PICTURE_NAME + Count.ToString();
    }
    #endregion
  }
}
