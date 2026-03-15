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
   /// Provides the WriteableBitmap context pixel data
   /// </summary>
   public static partial class WriteableBitmapContextExtensions
   {
      /// <summary>
      /// Gets a BitmapContext within which to perform nested IO operations on the bitmap
      /// </summary>
      /// <remarks>For WPF the BitmapContext will lock the bitmap. Call Dispose on the context to unlock</remarks>
      /// <param name="bmp"></param>
      /// <returns></returns>
      public static BitmapContext GetBitmapContext(this WriteableBitmap bmp)
      {
         return new BitmapContext(bmp);
      }

      /// <summary>
      /// Gets a BitmapContext within which to perform nested IO operations on the bitmap
      /// </summary>
      /// <remarks>For WPF the BitmapContext will lock the bitmap. Call Dispose on the context to unlock</remarks>
      /// <param name="bmp">The bitmap.</param>
      /// <param name="mode">The ReadWriteMode. If set to ReadOnly, the bitmap will not be invalidated on dispose of the context, else it will</param>
      /// <returns></returns>
      public static BitmapContext GetBitmapContext(this WriteableBitmap bmp, ReadWriteMode mode)
      {
         return new BitmapContext(bmp, mode);
      }
   }
}
