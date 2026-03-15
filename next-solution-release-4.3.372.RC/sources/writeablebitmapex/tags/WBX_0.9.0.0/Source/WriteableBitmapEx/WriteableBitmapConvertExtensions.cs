#region Header
//
//   Project:           WriteableBitmapEx - Silverlight WriteableBitmap extensions
//   Description:       Collection of interchange extension methods for the Silverlight WriteableBitmap class.
//
//   Changed by:        $Author$
//   Changed on:        $Date$
//   Changed in:        $Revision$
//   Project:           $URL$
//   Id:                $Id$
//
//
//   Copyright © 2009-2010 Rene Schulte and WriteableBitmapEx Contributors
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//
#endregion

namespace System.Windows.Media.Imaging
{
   /// <summary>
   /// Collection of interchange extension methods for the Silverlight WriteableBitmap class.
   /// </summary>
   public static partial class WriteableBitmapExtensions
   {
      #region Methods

      #region Byte Array

      /// <summary>
      /// Copies the Pixels from the WriteableBitmap into a ARGB byte array starting at a specific Pixels index.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="offset">The starting Pixels index.</param>
      /// <param name="count">The number of Pixels to copy.</param>
      /// <returns>The color buffer as byte ARGB values.</returns>
      public static byte[] ToByteArray(this WriteableBitmap bmp, int offset, int count)
      {
         int len = count * BufferStrideByteARGB;
         byte[] result = new byte[len]; // ARGB
         Buffer.BlockCopy(bmp.Pixels, offset, result, 0, len);
         return result;
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
         return bmp.ToByteArray(0, bmp.Pixels.Length);
      }

      /// <summary>
      /// Copies color information from an ARGB byte array into the WriteableBitmap starting at a specific buffer index.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="offset">The starting index in the buffer.</param>
      /// <param name="count">The number of bytes to copy from the buffer.</param>
      /// <param name="buffer">The color buffer as byte ARGB values.</param>
      public static void FromByteArray(this WriteableBitmap bmp, byte[] buffer, int offset, int count)
      {
         Buffer.BlockCopy(buffer, offset, bmp.Pixels, 0, count);
      }

      /// <summary>
      /// Copies color information from an ARGB byte array into the WriteableBitmap.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="count">The number of bytes to copy from the buffer.</param>
      /// <param name="buffer">The color buffer as byte ARGB values.</param>
      public static void FromByteArray(this WriteableBitmap bmp, byte[] buffer, int count)
      {
         bmp.FromByteArray(buffer, 0, count);
      }

      /// <summary>
      /// Copies all the color information from an ARGB byte array into the WriteableBitmap.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="buffer">The color buffer as byte ARGB values.</param>
      public static void FromByteArray(this WriteableBitmap bmp, byte[] buffer)
      {
         bmp.FromByteArray(buffer, 0, buffer.Length);
      }

      #endregion

      #region TGA File

      /// <summary>
      /// Writes the WriteableBitmap as a TGA image to a stream. 
      /// Used with permission from Nokola: http://nokola.com/blog/post/2010/01/21/Quick-and-Dirty-Output-of-WriteableBitmap-as-TGA-Image.aspx
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="destination">The destination stream.</param>
      public static void WriteTga(this WriteableBitmap bmp, System.IO.Stream destination)
      {
         int width = bmp.PixelWidth;
         int height = bmp.PixelHeight;
         int[] pixels = bmp.Pixels;
         byte[] data = new byte[pixels.Length * BufferStrideByteARGB];

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
               data[offsetDest]     = (byte)(color & 255);         // B
               data[offsetDest + 1] = (byte)((color >> 8) & 255);  // G
               data[offsetDest + 2] = (byte)((color >> 16) & 255); // R
               data[offsetDest + 3] = (byte)(color >> 24);         // A

               offsetSource++;
               offsetDest += BufferStrideByteARGB;
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
         using (var writer = new System.IO.BinaryWriter(destination))
         {
            writer.Write(header);
            writer.Write(data);
         }
      }


      #endregion

      #endregion
   }
}