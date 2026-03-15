#region Header
//
//   Project:           WriteableBitmapEx - WriteableBitmap extensions
//   Description:       Collection of extension methods for the WriteableBitmap class.
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2011-12-16 19:50:22 +0100 (Fr, 16 Dez 2011) $
//   Changed in:        $Revision: 83935 $
//   Project:           $URL: https://writeablebitmapex.svn.codeplex.com/svn/branches/WBX_1.0_BitmapContext/Source/WriteableBitmapEx/WriteableBitmapBaseExtensions.cs $
//   Id:                $Id: WriteableBitmapBaseExtensions.cs 83935 2011-12-16 18:50:22Z unknown $
//
//
//   Copyright © 2009-2012 Rene Schulte and WriteableBitmapEx Contributors
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//
#endregion

using System;

#if NETFX_CORE
namespace Windows.UI.Xaml.Media.Imaging
#else
namespace System.Windows.Media.Imaging
#endif
{
   /// <summary>
   /// Cross-platform factory for WriteableBitmaps
   /// </summary>
   public static class BitmapFactory
   {
      /// <summary>
      /// Creates a new WriteableBitmap of the specified width and height
      /// </summary>
      /// <remarks>For WPF the default DPI is 96x96 and PixelFormat is Pbgra32</remarks>
      /// <param name="pixelWidth"></param>
      /// <param name="pixelHeight"></param>
      /// <returns></returns>
      public static WriteableBitmap New(int pixelWidth, int pixelHeight)
      {
#if SILVERLIGHT
         return new WriteableBitmap(pixelWidth, pixelHeight);
#elif WPF
         return new WriteableBitmap(pixelWidth, pixelHeight, 96.0, 96.0, PixelFormats.Pbgra32, null);
#elif NETFX_CORE
         return new WriteableBitmap(pixelWidth, pixelHeight);
#endif
      }

#if WPF
      /// <summary>
      /// Converts the input BitmapSource to the Pbgra32 format WriteableBitmap which is internally used by the WriteableBitmapEx.
      /// </summary>
      /// <param name="source">The source bitmap.</param>
      /// <returns></returns>
      public static WriteableBitmap ConvertToPbgra32Format(BitmapSource source)
      {
         // Convert to Pbgra32 if it's a different format
         if (source.Format == PixelFormats.Pbgra32)
         {
            return new WriteableBitmap(source);
         }

         var formatedBitmapSource = new FormatConvertedBitmap();
         formatedBitmapSource.BeginInit();
         formatedBitmapSource.Source = source;
         formatedBitmapSource.DestinationFormat = PixelFormats.Pbgra32;
         formatedBitmapSource.EndInit();
         return new WriteableBitmap(formatedBitmapSource);
      }
#endif
   }
}