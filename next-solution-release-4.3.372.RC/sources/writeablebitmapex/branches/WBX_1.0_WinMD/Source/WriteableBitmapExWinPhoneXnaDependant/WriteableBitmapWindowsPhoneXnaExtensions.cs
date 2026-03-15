#region Header
//
//   Project:           WriteableBitmapEx - Silverlight WriteableBitmap extensions
//   Description:       Collection of draw spline extension methods for the Silverlight WriteableBitmap class.
//
//   Changed by:        $Author: unknown $
//   Changed on:        $Date: 2010-09-17 22:20:49 +0200 (Fr, 17 Sep 2010) $
//   Changed in:        $Revision: 61101 $
//   Project:           $URL: https://writeablebitmapex.svn.codeplex.com/svn/trunk/Source/WriteableBitmapEx/WriteableBitmapSplineExtensions.cs $
//   Id:                $Id: WriteableBitmapSplineExtensions.cs 61101 2010-09-17 20:20:49Z unknown $
//
//
//   Copyright © 2009-2010 Rene Schulte and WriteableBitmapEx Contributors
//
//   This Software is weak copyleft open source. Please read the License.txt for details.
//
#endregion

using System.IO;
using Microsoft.Xna.Framework.Media;

namespace System.Windows.Media.Imaging
{
   /// <summary>
   /// Collection of draw spline extension methods for the Silverlight WriteableBitmap class.
   /// </summary>
   public static partial class WriteableBitmapExtensions
   {
      #region Methods

      /// <summary>
      /// Saves the WriteableBitmap encoded as JPEG to the Media library using the best quality of 100.
      /// </summary>
      /// <param name="bitmap">The WriteableBitmap to save.</param>
      /// <param name="name">The name of the destination file.</param>
      /// <param name="saveToCameraRoll">If true the bitmap will be saved to the camera roll, otherwise it will be written to the default saved album.</param>
      public static void SaveToMediaLibrary(this WriteableBitmap bitmap, string name, bool saveToCameraRoll = false)
      {
         SaveToMediaLibrary(bitmap, name, 100, saveToCameraRoll);
      }

      /// <summary>
      /// Saves the WriteableBitmap encoded as JPEG to the Media library.
      /// </summary>
      /// <param name="bitmap">The WriteableBitmap to save.</param>
      /// <param name="name">The name of the destination file.</param>
      /// <param name="quality">The quality for JPEG encoding has to be in the range 0-100, 
      /// where 100 is the best quality with the largest size.</param>
      /// <param name="saveToCameraRoll">If true the bitmap will be saved to the camera roll, otherwise it will be written to the default saved album.</param>
      public static void SaveToMediaLibrary(this WriteableBitmap bitmap, string name, int quality, bool saveToCameraRoll = false)
      {
         using (var stream = new MemoryStream())
         {
            // Save the picture to the WP7 media library
            bitmap.SaveJpeg(stream, bitmap.PixelWidth, bitmap.PixelHeight, 0, quality);
            stream.Seek(0, SeekOrigin.Begin);
            if (saveToCameraRoll)
            {
               new MediaLibrary().SavePictureToCameraRoll(name, stream);               
            }
            else
            {
               new MediaLibrary().SavePicture(name, stream);
            }
         }
      }
      
      #endregion
   }
}