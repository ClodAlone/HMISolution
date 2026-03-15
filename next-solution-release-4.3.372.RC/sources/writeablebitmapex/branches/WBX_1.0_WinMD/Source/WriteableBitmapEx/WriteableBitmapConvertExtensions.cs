#region Header
//
//   Project:           WriteableBitmapEx - WriteableBitmap extensions
//   Description:       Collection of interchange extension methods for the WriteableBitmap class.
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2012-05-05 20:52:49 +0200 (Sa, 05 Mai 2012) $
//   Changed in:        $Revision: 90091 $
//   Project:           $URL: https://writeablebitmapex.svn.codeplex.com/svn/branches/WBX_1.0_BitmapContext/Source/WriteableBitmapEx/WriteableBitmapConvertExtensions.cs $
//   Id:                $Id: WriteableBitmapConvertExtensions.cs 90091 2012-05-05 18:52:49Z unknown $
//
//
//   Copyright © 2009-2012 Rene Schulte and WriteableBitmapEx Contributors
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//
#endregion

using System;
using System.IO;
using System.Reflection;

#if NETFX_CORE
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

namespace Windows.UI.Xaml.Media.Imaging
#else
namespace System.Windows.Media.Imaging
#endif
{
   /// <summary>
   /// Collection of interchange extension methods for the WriteableBitmap class.
   /// </summary>
   public
#if WPF
 unsafe
#endif
 static partial class WriteableBitmapExtensions
   {
      #region Methods

      #region Byte Array

      /// <summary>
      /// Copies the Pixels from the WriteableBitmap into a ARGB byte array starting at a specific Pixels index.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="offset">The starting Pixels index.</param>
      /// <param name="count">The number of Pixels to copy, -1 for all</param>
      /// <returns>The color buffer as byte ARGB values.</returns>
      public static byte[] ToByteArray(this WriteableBitmap bmp, int offset, int count)
      {
         using (var context = bmp.GetBitmapContext())
         {
            if (count == -1)
            {
               // Copy all to byte array
               count = context.Length;
            }

            int len = count * SizeOfArgb;
            byte[] result = new byte[len]; // ARGB
            BitmapContext.BlockCopy(context, offset, result, 0, len);
            return result;
         }
      }

      /// <summary>
      /// Copies the Pixels from the WriteableBitmap into a ARGB byte array.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="count">The number of pixels to copy.</param>
      /// <returns>The color buffer as byte ARGB values.</returns>
      public static byte[] ToByteArray(this WriteableBitmap bmp, int count)
      {
         return bmp.ToByteArray(0, count);
      }

      /// <summary>
      /// Copies all the Pixels from the WriteableBitmap into a ARGB byte array.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <returns>The color buffer as byte ARGB values.</returns>
      public static byte[] ToByteArray(this WriteableBitmap bmp)
      {
         return bmp.ToByteArray(0, -1);
      }

      /// <summary>
      /// Copies color information from an ARGB byte array into this WriteableBitmap starting at a specific buffer index.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="offset">The starting index in the buffer.</param>
      /// <param name="count">The number of bytes to copy from the buffer.</param>
      /// <param name="buffer">The color buffer as byte ARGB values.</param>
      /// <returns>The WriteableBitmap that was passed as parameter.</returns>
      public static WriteableBitmap FromByteArray(this WriteableBitmap bmp, byte[] buffer, int offset, int count)
      {
         using (var context = bmp.GetBitmapContext())
         {
            BitmapContext.BlockCopy(buffer, offset, context, 0, count);
            return bmp;
         }
      }

      /// <summary>
      /// Copies color information from an ARGB byte array into this WriteableBitmap.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="count">The number of bytes to copy from the buffer.</param>
      /// <param name="buffer">The color buffer as byte ARGB values.</param>
      /// <returns>The WriteableBitmap that was passed as parameter.</returns>
      public static WriteableBitmap FromByteArray(this WriteableBitmap bmp, byte[] buffer, int count)
      {
         return bmp.FromByteArray(buffer, 0, count);
      }

      /// <summary>
      /// Copies all the color information from an ARGB byte array into this WriteableBitmap.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="buffer">The color buffer as byte ARGB values.</param>
      /// <returns>The WriteableBitmap that was passed as parameter.</returns>
      public static WriteableBitmap FromByteArray(this WriteableBitmap bmp, byte[] buffer)
      {
         return bmp.FromByteArray(buffer, 0, buffer.Length);
      }

      #endregion

      #region TGA File

      /// <summary>
      /// Writes the WriteableBitmap as a TGA image to a stream. 
      /// Used with permission from Nokola: http://nokola.com/blog/post/2010/01/21/Quick-and-Dirty-Output-of-WriteableBitmap-as-TGA-Image.aspx
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="destination">The destination stream.</param>
#if WINMD
      public static void WriteTga(this WriteableBitmap bmp, IRandomAccessStream destination)
#else
      public static void WriteTga(this WriteableBitmap bmp, Stream destination)
#endif
      {
         using (var context = bmp.GetBitmapContext())
         {
            int width = context.Width;
            int height = context.Height;
            var pixels = context.Pixels;
            byte[] data = new byte[context.Length * SizeOfArgb];

            // Copy bitmap data as BGRA
            int offsetSource = 0;
            int width4 = width << 2;
            int width8 = width << 3;
            int offsetDest = (height - 1) * width4;
            for (int y = 0; y < height; y++)
            {
               for (int x = 0; x < width; x++)
               {
                  int color = pixels[offsetSource];
                  data[offsetDest] = (byte)(color & 255);         // B
                  data[offsetDest + 1] = (byte)((color >> 8) & 255);  // G
                  data[offsetDest + 2] = (byte)((color >> 16) & 255); // R
                  data[offsetDest + 3] = (byte)(color >> 24);         // A

                  offsetSource++;
                  offsetDest += SizeOfArgb;
               }
               offsetDest -= width8;
            }

            // Create header
            byte[] header = new byte[]
         {
            0, // ID length
            0, // no color map
            2, // uncompressed, true color
            0, 0, 0, 0,
            0,
            0, 0, 0, 0, // x and y origin
            (byte)(width & 0x00FF),
            (byte)((width & 0xFF00) >> 8),
            (byte)(height & 0x00FF),
            (byte)((height & 0xFF00) >> 8),
            32, // 32 bit bitmap
            0 
         };

            // Write header and data
#if WINMD
            var stream = destination.AsStreamForWrite();
#else
            var stream = destination;
#endif
            using (var writer = new BinaryWriter(stream))
            {
               writer.Write(header);
               writer.Write(data);
            }
         }
      }

      #endregion

      #region Resource
#if !NETFX_CORE
      /// <summary>
      /// Loads an image from the applications resource file and fills this WriteableBitmap with it.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="relativePath">Only the relative path to the resource file. The assembly name is retrieved automatically.</param>
      /// <returns>The WriteableBitmap that was passed as parameter.</returns>
      public static WriteableBitmap FromResource(this WriteableBitmap bmp, string relativePath)
      {
         var fullName = Assembly.GetCallingAssembly().FullName;
         var asmName = new AssemblyName(fullName).Name;
         return bmp.FromContent(asmName + ";component/" + relativePath);
      }
#endif

#if NETFX_CORE
      /// <summary>
      /// Loads an image from the applications content and fills this WriteableBitmap with it.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="uri">The URI to the content file.</param>
      /// <returns>The WriteableBitmap that was passed as parameter.</returns>
#if WINMD
      public static IAsyncOperation<WriteableBitmap> FromContent(this WriteableBitmap bmp, Uri uri)
      {
         return (IAsyncOperation<WriteableBitmap>)AsyncInfo.Run(ct => FromContentWinRT(bmp, uri));
      }
#else
      public static async Task<WriteableBitmap> FromContent(this WriteableBitmap bmp, Uri uri)
      {
         return await FromContentWinRT(bmp, uri);
      }
#endif
      private static async Task<WriteableBitmap> FromContentWinRT(this WriteableBitmap bmp, Uri uri)
      {
         // Decode pixel data
         var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
         var decoder = await BitmapDecoder.CreateAsync(await file.OpenAsync(FileAccessMode.Read));
         var transform = new BitmapTransform();
         var pixelData = await decoder.GetPixelDataAsync(decoder.BitmapPixelFormat, decoder.BitmapAlphaMode, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.ColorManageToSRgb);

         // Swap R and B channels
         var pixels = pixelData.DetachPixelData();
         for (var i = 0; i < pixels.Length; i += 4)
         {
            var r = pixels[i];
            var b = pixels[i + 2];
            pixels[i] = b;
            pixels[i + 2] = r;
         }

         // Copy to WriteableBitmap
         bmp = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
         using (var bmpStream = bmp.PixelBuffer.AsStream())
         {
            bmpStream.Seek(0, SeekOrigin.Begin);
            bmpStream.Write(pixels, 0, (int)bmpStream.Length);
            return bmp;
         }

#else
      /// <summary>
      /// Loads an image from the applications content and fills this WriteableBitmap with it.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="relativePath">Only the relative path to the content file.</param>
      /// <returns>The WriteableBitmap that was passed as parameter.</returns>
       public static WriteableBitmap FromContent(this WriteableBitmap bmp, string relativePath)
      {
        using (var bmpStream = Application.GetResourceStream(new Uri(relativePath, UriKind.Relative)).Stream)
         {
            var bmpi = new BitmapImage();
#if SILVERLIGHT
            bmpi.SetSource(bmpStream);
            bmpi.CreateOptions = BitmapCreateOptions.None;
#elif WPF
            bmpi.StreamSource = bmpStream;
#endif
            bmp = new WriteableBitmap(bmpi);
            bmpi.UriSource = null;
            return bmp;
         }
#endif
      }

      #endregion

      #endregion
   }
}