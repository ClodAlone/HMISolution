#region Header
//
//   Project:           WriteableBitmapEx - Silverlight WriteableBitmap extensions
//   Description:       Collection of transformation extension methods for the Silverlight WriteableBitmap class.
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
   /// Collection of filter / convolution extension methods for the Silverlight WriteableBitmap class.
   /// </summary>
   public static partial class WriteableBitmapExtensions
   {
      #region Kernels

      ///<summary>
      /// Gaussian blur kernel with the size 5x5
      ///</summary>
      public static int[,] KernelGaussianBlur5x5 = {
                                                       {1,  4,  7,  4, 1},
                                                       {4, 16, 26, 16, 4},
                                                       {7, 26, 41, 26, 7},
                                                       {4, 16, 26, 16, 4},
                                                       {1,  4,  7,  4, 1}
                                                 };

      ///<summary>
      /// Gaussian blur kernel with the size 3x3
      ///</summary>
      public static int[,] KernelGaussianBlur3x3 = {
                                                       {16, 26, 16},
                                                       {26, 41, 26},
                                                       {16, 26, 16}
                                                    };

      ///<summary>
      /// Sharpen kernel with the size 3x3
      ///</summary>
      public static int[,] KernelSharpen3x3 = {
                                                 { 0, -2,  0},
                                                 {-2, 11, -2},
                                                 { 0, -2,  0}
                                              };

      #endregion

      #region Methods

      #region Convolute

      /// <summary>
      /// Creates a new filtered WriteableBitmap.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="kernel">The kernel used for convolution.</param>
      /// <returns>A new WriteableBitmap that is a filtered version of the input.</returns>
      public static WriteableBitmap Convolute(this WriteableBitmap bmp, int[,] kernel)
      {
         var kernelFactorSum = 0;
         foreach (var b in kernel)
         {
            kernelFactorSum += b;
         }
         return bmp.Convolute(kernel, kernelFactorSum, 0);
      }

      /// <summary>
      /// Creates a new filtered WriteableBitmap.
      /// </summary>
      /// <param name="bmp">The WriteableBitmap.</param>
      /// <param name="kernel">The kernel used for convolution.</param>
      /// <param name="kernelFactorSum">The factor used for the kernel summing.</param>
      /// <param name="kernelOffsetSum">The offset used for the kernel summing.</param>
      /// <returns>A new WriteableBitmap that is a filtered version of the input.</returns>
      public static WriteableBitmap Convolute(this WriteableBitmap bmp, int[,] kernel, int kernelFactorSum, int kernelOffsetSum)
      {
         var kh = kernel.GetUpperBound(0) + 1;
         var kw = kernel.GetUpperBound(1) + 1;

         if ((kw & 1) == 0)
         {
            throw new InvalidOperationException("Kernel width must be odd!");
         }
         if ((kh & 1) == 0)
         {
            throw new InvalidOperationException("Kernel height must be odd!");
         }

         var pixels = bmp.Pixels;
         var w = bmp.PixelWidth;
         var h = bmp.PixelHeight;
         var result = new WriteableBitmap(w, h);
         var resultPixels = result.Pixels;
         var index = 0;
         var kwh = kw >> 1;
         var khh = kh >> 1;

         for (var y = 0; y < h; y++)
         {
            for (var x = 0; x < w; x++)
            {
               var a = 0;
               var r = 0;
               var g = 0;
               var b = 0;

               for (var kx = -kwh; kx <= kwh; kx++)
               {
                  var px = kx + x;
                  // Repeat pixels at borders
                  if (px < 0)
                  {
                     px = 0;
                  }
                  else if (px >= w)
                  {
                     px = w - 1;
                  }

                  for (var ky = -khh; ky <= khh; ky++)
                  {
                     var py = ky + y;
                     // Repeat pixels at borders
                     if (py < 0)
                     {
                        py = 0;
                     }
                     else if (py >= h)
                     {
                        py = h - 1;
                     }

                     var col = pixels[py * w + px];
                     var k = kernel[ky + kwh, kx + khh];
                     a += ((col >> 24) & 0x000000FF) * k;
                     r += ((col >> 16) & 0x000000FF) * k;
                     g += ((col >>  8) & 0x000000FF) * k;
                     b += ((col)       & 0x000000FF) * k;
                  }
               }

               var ba = (byte)((a / kernelFactorSum) + kernelOffsetSum);
               var br = (byte)((r / kernelFactorSum) + kernelOffsetSum);
               var bg = (byte)((g / kernelFactorSum) + kernelOffsetSum);
               var bb = (byte)((b / kernelFactorSum) + kernelOffsetSum);

               resultPixels[index++] = (ba << 24) | (br << 16) | (bg << 8) | (bb);
            }
         }

         return result;
      }

      #endregion

      #endregion
   }
}