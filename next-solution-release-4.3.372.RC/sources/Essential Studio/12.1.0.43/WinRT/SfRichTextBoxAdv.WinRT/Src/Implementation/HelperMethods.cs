#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Net;
using System.Text;
#if WPF
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
#else
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal static class HelperMethods
    {
        /// <summary>
        /// Finds whether the two rectangle intersects each other
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="rect2"></param>
        /// <returns></returns>
        internal static bool IsIntersecting(this Rect rect, Rect rect1)
        {
            rect.Intersect(rect1);

            return !rect.IsEmpty;
        }
        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        internal static byte[] GetBytes(this Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            byte[] bytearray = new byte[stream.Length];
            stream.Read(bytearray, 0, bytearray.Length);
            return bytearray;
        }
        /// <summary>
        /// To the memory stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        internal static MemoryStream ToMemoryStream(this Stream stream)
        {
            MemoryStream result = new MemoryStream();
            stream.CopyTo(result);
            result.Seek(0L, SeekOrigin.Begin);
            return result;
        }
        /// <summary>
        /// Sets the source.
        /// </summary>
        /// <param name="bitmapImage">The bitmap image.</param>
        /// <param name="imageStream">The image stream.</param>
        internal static void SetSource(this BitmapImage bitmapImage, MemoryStream imageStream)
        {
#if WPF 
            bitmapImage.StreamSource = imageStream.ToMemoryStream();
#else
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            Stream stream = randomAccessStream.AsStreamForWrite();
            imageStream.Position = 0;
            //Copies the data to random access stream.
            imageStream.CopyTo(stream);
            stream.Position = 0;
            bitmapImage.SetSource(randomAccessStream);
#endif
        }

#if !SyncfusionFramework4_0
        /// <summary>
        /// Copies the data to output stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="output">The output.</param>
        internal static void CopyTo(this Stream stream, Stream output)
        {
            const int bufferSize = 0x1000;
            int num;
            byte[] buffer = new byte[bufferSize];
            while ((num = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                output.Write(buffer, 0, num);
            }
        }
        /// <summary>
        /// Clears the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        internal static void Clear(this StringBuilder builder)
        {
            if (builder.Length > 0)
            {
                builder.Remove(0, builder.Length);
            }
        }
#endif
    }

    internal class UIDispatcher
    {
#if WPF
        /// <summary>
        /// Executes the action using UIElement.
        /// </summary>
        /// <param name="action">An Action.</param>
        //internal static void Execute(Action action)
        //{
        //    action();
        //}
#else
        /// <summary>
        /// Gets the core dispatcher.
        /// </summary>
        private static CoreDispatcher GetCoreDispatcher()
        {
            CoreWindow coreWindow = null;
            try
            {
                coreWindow = CoreApplication.MainView.CoreWindow;
            }
            catch
            { }
            if (coreWindow == null)
            {
                CoreApplicationView coreApplicationView = null;
                try
                {
                    coreApplicationView = CoreApplication.GetCurrentView();
                }
                catch
                { }
                if (coreApplicationView != null && coreApplicationView.CoreWindow is CoreWindow)
                    return coreApplicationView.CoreWindow.Dispatcher;
                return null;
            }
            return coreWindow.Dispatcher;
        }
        /// <summary>
        /// Executes the action using UIElement.
        /// </summary>
        /// <param name="action">An Action.</param>
        internal static void Execute(Action action)
        {
            CoreDispatcher dispatcher = GetCoreDispatcher();
            if (dispatcher == null || dispatcher.HasThreadAccess)
                action();
            else
                dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action()).AsTask().Wait();
        }
#endif
    }
}
