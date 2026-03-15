#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Syncfusion.RDL.Internal
{
    /// <summary>
    /// Interaction logic for ImageConversion.xaml
    /// </summary>
    internal partial class ImageConversion : UserControl
    {
        Canvas InnerCanvas;
       
        /// <summary>
        /// Conversion of Control to Image
        /// </summary>
        /// <param name="innerControl">Input as Control Method</param>
        /// <returns>Memory Stream of Image</returns>
        public MemoryStream CovertToImage(Control innerControl)
        {
            try
            {
                this.InnerCanvas = new Canvas();
                this.Content = this.InnerCanvas;

                innerControl.Margin = new Thickness(0);
                InnerCanvas.Children.Add(innerControl);

                InnerCanvas.Width = innerControl.Width;
                InnerCanvas.Height = innerControl.Height;

                Canvas canvas = this.InnerCanvas;
                canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
                canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

                int Height = ((int)(InnerCanvas.ActualHeight));
                int Width = ((int)(InnerCanvas.ActualWidth));
#if !MVC
                Height=(int)((Height * 300) / 96);
                Width = (int)((Width * 300) / 96);
#endif

                this.Height = InnerCanvas.Height;
                this.Width = InnerCanvas.Width;
                InnerCanvas.LayoutTransform = null;

                Size size = new Size(InnerCanvas.ActualWidth, InnerCanvas.ActualHeight);

                InnerCanvas.Background = Brushes.White;
                InnerCanvas.Arrange(new Rect(size));
                InnerCanvas.UpdateLayout();
#if MVC
                RenderTargetBitmap rtb = new RenderTargetBitmap(Width, Height, 96, 96, PixelFormats.Default);
#else

                RenderTargetBitmap rtb = new RenderTargetBitmap(Width, Height, 300, 300, PixelFormats.Default);
#endif
                rtb.Render(InnerCanvas);

                var Source = new MemoryStream();
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(Source);
                return Source;
            }
            catch
            {
                return null;
            }
        }
    }
}
