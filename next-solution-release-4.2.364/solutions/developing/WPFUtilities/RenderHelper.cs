using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Utilities
{
    public class RenderHelper
    {
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static BitmapSource ElementToGrayscaleBitmap(FrameworkElement e)
        {
            if (e.ActualWidth == 0 || e.ActualHeight == 0) return null;
            return VisualToBitmap(e, (int)e.ActualWidth, (int)e.ActualHeight, new GrayscaleParameters());
        }

        public static BitmapSource ElementToBitmap(FrameworkElement e)
        {
            if (e.ActualWidth == 0 || e.ActualHeight == 0) return null;
            return VisualToBitmap(e, (int)e.ActualWidth, (int)e.ActualHeight, null);
        }

        public static BitmapSource ElementToBitmap(FrameworkElement e, GrayscaleParameters parameters)
        {
            if (e.ActualWidth == 0 || e.ActualHeight == 0) return null;
            return VisualToBitmap(e, (int)e.ActualWidth, (int)e.ActualHeight, parameters);
        }

        public static BitmapSource VisualToBitmap(Visual e, int Width, int Height)
        {
            return VisualToBitmap(e, Width, Height, null);
        }

        public static BitmapSource VisualToBitmap(Visual e, int Width, int Height, GrayscaleParameters parameters)
        {
            if (e == null) return null;

            RenderTargetBitmap src = new RenderTargetBitmap(Width, Height, 96, 96, PixelFormats.Pbgra32);
            src.Render(e);

            if (parameters != null)
            {
                byte[] pixels = new byte[Width * Height * 4];
                src.CopyPixels(pixels, (Width * 4), 0);

                for (int p = 0; p < pixels.Length; p += 4)
                {

                    // compute grayscale
                    double pixelvalue =
                        (((double)pixels[p + 0] * parameters.RedDistribution) +
                        ((double)pixels[p + 1] * parameters.GreenDistribution) +
                        ((double)pixels[p + 2] * parameters.BlueDistribution));

                    // compute compression
                    pixelvalue = (pixelvalue * parameters.Compression) + (256 * ((1 - parameters.Compression) / 2));

                    // compute washout
                    pixelvalue = Math.Min(256, pixelvalue + (256 * parameters.Washout));

                    byte v = (byte)pixelvalue;
                    pixels[p + 0] = v;
                    pixels[p + 1] = v;
                    pixels[p + 2] = v;
                }

                return BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, pixels, Width * 4);
            }
            else
            {
                return src;
            }
        }

        public static string VisualToFile(Visual e, int Width, int Height, GrayscaleParameters parameters, string Filename)
        {
            BitmapSource src = VisualToBitmap(e, Width, Height, parameters);

            using (System.IO.FileStream fs = new System.IO.FileStream(Filename, System.IO.FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(src));
                encoder.Save(fs);
            }

            return Filename;
        }

        public static object LoadTemplate(System.IO.Stream templateStream, string templateName)
        {
            try
            {
                if (templateStream == null)
                    return null;

                ResourceDictionary obj = System.Windows.Markup.XamlReader.Load(templateStream) as ResourceDictionary;
                if (obj == null)
                    return null;
                System.Windows.Controls.Canvas canvas = new System.Windows.Controls.Canvas();
                canvas.Resources.MergedDictionaries.Add(obj);
                object content = canvas.TryFindResource(templateName);
                return content;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

    public class GrayscaleParameters
    {
        private double _RedDistribution = 0.30;
        public double RedDistribution
        {
            get { return _RedDistribution; }
            set { _GreenDistribution = Math.Max(0, Math.Min(1, value)); }
        }

        private double _GreenDistribution = 0.59;
        public double GreenDistribution
        {
            get { return _GreenDistribution; }
            set { _GreenDistribution = Math.Max(0, Math.Min(1, value)); }
        }

        private double _BlueDistribution;
        public double BlueDistribution
        {
            get { return _BlueDistribution = 0.11; }
            set { _BlueDistribution = Math.Max(0, Math.Min(1, value)); }
        }

        private double _Compression = 0.8;
        public double Compression
        {
            get { return _Compression; }
            set { _Compression = Math.Max(0, Math.Min(1, value)); }
        }

        private double _Washout = -0.05;
        public double Washout
        {
            get { return _Washout; }
            set { _Washout = Math.Max(0, Math.Min(1, value)); }
        }
    }
}
