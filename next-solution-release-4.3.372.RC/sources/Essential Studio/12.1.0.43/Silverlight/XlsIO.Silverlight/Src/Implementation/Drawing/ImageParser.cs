#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Parser.Biff_Records;

namespace Syncfusion.XlsIO.Implementation.Silverlight
{
  public static class ImageParser
  {
    #region Constants
    /// <summary>
    /// Maximum signature length.
    /// </summary>
    private static int MaxSignatureLength;
    /// <summary>
    /// PNG file signature.
    /// </summary>
    private static readonly byte[] PngSignature = new byte[]
    {
      137, 80, 78, 71, 13, 10, 26, 10,
    };
    /// <summary>
    /// Bitmap signature.
    /// </summary>
    private static readonly byte[] BmpSignature = new byte[]
    {
      0x42, 0x4d,
    };
    /// <summary>
    /// Jpeg signature.
    /// </summary>
    private static readonly byte[] JpegSignature = new byte[]
    {
      0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46,
    };
    /// <summary>
    /// Signature list. Order must be same as in ImageType enumeration.
    /// </summary>
    private static readonly byte[][] Signatures = new byte[][]
    {
      JpegSignature,
      BmpSignature,
      PngSignature,
      null,
    };
    public enum ImageType
    {
      Unknown = -1,
      Jpeg,
      Bitmap,
      Png,
      Metafile,
    }
    #endregion

    #region Methods
    static ImageParser()
    {
      EvaluateMaxSignatureLength();
    }
    /// <summary>
    /// Determines type of the passes image.
    /// </summary>
    /// <param name="stream">Stream containing image.</param>
    /// <returns>Detected image type.</returns>
    public static ImageType DetectImageType( Stream stream )
    {
      byte[] header = new byte[ MaxSignatureLength ];

      if( stream.Read( header, 0, MaxSignatureLength ) != MaxSignatureLength )
        throw new ApplicationException();

      stream.Position -= MaxSignatureLength;

      ImageType result = ImageType.Unknown;

      for( int i = Signatures.Length - 1; i >= 0; i-- )
      {
        byte[] currentSignature = Signatures[ i ];

        if( currentSignature != null &&
          BiffRecordRaw.CompareArrays( currentSignature, 0, header, 0, currentSignature.Length ) )
        {
          result = ( ImageType )i;
          break;
        }
      }

      return result;
    }
    public static Size GetImageSize( Stream stream )
    {
      throw new NotImplementedException();
    }
    public static byte[] GetImageData( Stream stream )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Evaluates maximum known signature length.
    /// </summary>
    private static void EvaluateMaxSignatureLength()
    {
      if( MaxSignatureLength == 0 )
      {
        for( int i = Signatures.Length - 1; i >= 0; i-- )
        {
          byte[] currentSignature = Signatures[ i ];
          int iSignatureLen = ( currentSignature != null ) ?
            currentSignature.Length :
            0;

          MaxSignatureLength = Math.Max( MaxSignatureLength, iSignatureLen );
        }
      }
    }
    #endregion
  }
}
