#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if !(WINDOWS_PHONE_7 || Silverlight4)
using System.Threading.Tasks;
#endif
#if !(SILVERLIGHT||WPF||WINDOWS_PHONE_7)
using System.Runtime.InteropServices.WindowsRuntime;
#endif
using System.Text;

#if WINDOWS_PHONE ||WINDOWS_PHONE_7
using System.Windows.Media.Imaging;

namespace Syncfusion.WP.Controls
{
#else
#if SILVERLIGHT
using System.Windows.Media.Imaging;
namespace Syncfusion.Tools.Controls
{
#else
#if WPF
using System.Windows.Media.Imaging;
namespace Syncfusion.Windows.Controls
{
#else
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Syncfusion.UI.Xaml.Controls
{
#endif
#endif
#endif
    /// <summary>
    /// Represents a class for Bitmap Extensions
    /// </summary>
   public static class WriteableBitmapExtention
    {
       /// <summary>
        /// Validate the saturation values
       /// </summary>
       /// <param name="target"></param>
       /// <param name="hue"></param>
        public static void RenderSaturationValue(this WriteableBitmap target, double hue = 0)
        {
            var pw = target.PixelWidth;
#if !(WINDOWS_PHONE_7||WINDOWS_PHONE)
            var hw = pw / 2;
#else 
            var hw=0;
#endif
            var ph = target.PixelHeight;
            var invPh = 1.0 / ph;
#if WINDOWS_PHONE||WINDOWS_PHONE_7||SILVERLIGHT
            var pixels = target.Pixels.GetPixels();
            ColorPickerHelper.RenderSaturationValueCore(hue, ph, hw, invPh, pw, pixels);
#else
#if WPF
            PixelBufferInfo[] pixels = null;
            target.CopyPixels(pixels, 0, 0);
            ColorPickerHelper.RenderSaturationValueCore(hue, ph, hw, invPh, pw, pixels[0]);
#else
            var pixels=target.PixelBuffer.GetPixels();
            ColorPickerHelper.RenderSaturationValueCore(hue, ph, hw, invPh, pw, pixels);
#endif
#endif

#if !WPF
            target.Invalidate();
#endif
        }
#if !(WPFSILVERLIGHT||WINDOWS_PHONE_7)
       /// <summary>
       /// Validate the saturation values
       /// </summary>
       /// <param name="target"></param>
       /// <param name="hue"></param>
       /// <returns></returns>
        public static async Task RenderSaturationValueAsync(this WriteableBitmap target, double hue = 0)
        {
            var pw = target.PixelWidth;
            var ph = target.PixelHeight;
            var invPh = 1.0 / ph;
#if !WINRT
            var pixels = target.Pixels.GetPixels();
#else
            var pixels = target.PixelBuffer.GetPixels();
#endif
            await Task.Run(() => ColorPickerHelper.RenderSaturationValueCore(hue, ph, 0, invPh, pw, pixels));
            target.Invalidate();
        }
#endif
        /// <summary>
       /// Retrieves information of the pixel buffer
       /// </summary>
       /// <param name="pixelBuffer"></param>
       /// <returns></returns>
#if !WINRT
        public static PixelBufferInfo GetPixels(this int[] pixelBuffer)
#else
        public static PixelBufferInfo GetPixels(this IBuffer pixelBuffer)
#endif
        {
            return new PixelBufferInfo(pixelBuffer);
        }
    }
    

}
