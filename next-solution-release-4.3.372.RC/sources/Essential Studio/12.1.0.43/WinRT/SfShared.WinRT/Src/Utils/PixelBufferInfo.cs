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
#if !(SILVERLIGHT||WPF||WINDOWS_PHONE_7)
using System.Runtime.InteropServices.WindowsRuntime;
#endif
using System.Text;

#if WINDOWS_PHONE||WINDOWS_PHONE_7

namespace Syncfusion.WP.Controls
{
#else
#if SILVERLIGHT
namespace Syncfusion.Tools.Controls
{
#else
#if WPF
namespace Syncfusion.Windows.Controls
{
#else
using Windows.Storage.Streams;

namespace Syncfusion.UI.Xaml.Controls
{
#endif
#endif
#endif
    /// <summary>
    /// Represents a class to maintain the pixel buffer Information
    /// </summary>
        public class PixelBufferInfo
        {
#if !WINRT
            /// <summary>
            /// Declares a variable for pixels
            /// </summary>
            public int[] pixels;
            /// <summary>
            /// Gets or sets the pixels
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public int this[int i]
            {
                get
                {
                    return pixels[i];
                }
                set
                {
                    pixels[i] = value;
                }
            }
#else
            private readonly Stream _pixelStream;
            
            /// <summary>
            /// Declares a variable for Bytes
            /// </summary>
            public byte[] Bytes;

            /// <summary>
            /// Gets or sets the Bytes
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public int this[int i]
            {
                get
                {
                    return ColorPickerHelper.IntColorFromBytes(
                        Bytes[i*4 + 3],
                        Bytes[i*4 + 2],
                        Bytes[i*4 + 1],
                        Bytes[i*4 + 0]);
                }
                set
                {
                    Bytes[i*4 + 3] = (byte) ((value >> 24) & 0xff);
                    Bytes[i*4 + 2] = (byte) ((value >> 16) & 0xff);
                    Bytes[i*4 + 1] = (byte) ((value >> 8) & 0xff);
                    Bytes[i*4 + 0] = (byte) ((value) & 0xff);
                    _pixelStream.Seek(i*4, SeekOrigin.Begin);
                    _pixelStream.Write(Bytes, i*4, 4);
                }
            }
#endif

            /// <summary>
            /// Retieves information about the pixel buffer
            /// </summary>
            /// <param name="pixelBuffer"></param>
#if !WINRT
            public PixelBufferInfo(int[] pixelBuffer)
            {
                this.pixels = pixelBuffer; 
            }
#else
            public PixelBufferInfo(IBuffer pixelBuffer)
            {
                _pixelStream = pixelBuffer.AsStream();
                this.Bytes = new byte[_pixelStream.Length];
                _pixelStream.Seek(0, SeekOrigin.Begin);
                _pixelStream.Read(this.Bytes, 0, Bytes.Length);
            }

            /// <summary>
            /// Seek and Write bytes
            /// </summary>
            public void UpdateFromBytes()
            {
                _pixelStream.Seek(0, SeekOrigin.Begin);
                _pixelStream.Write(Bytes, 0, Bytes.Length);
            }
#endif
        }
    }