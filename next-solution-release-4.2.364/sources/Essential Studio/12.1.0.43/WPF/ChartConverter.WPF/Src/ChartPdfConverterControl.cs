#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion


namespace Syncfusion.Windows.Chart.Converter
{

    using System;
    using System.Windows;
    using Syncfusion.Pdf.Graphics;
    using Syncfusion.Windows.Chart;
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Documents;
    using System.Windows.Controls;
    using System.Text;
    using System.Windows.Markup;
    using Syncfusion.Pdf;
    using System.Windows.Shapes;

    /// <summary>
    /// Converter Base includes the basic properties and methods used for exporting Chart to PDF
    /// </summary>
    [CLSCompliant(false)]
    public class ChartPdfConverterControl
    {
        public void ChartPdfConverter(Chart chart, string pdfFile)
        {
            //Chart chart = new Chart();
            Visual visual = chart.Template.FindName("PART_INTERNAL_BORDER", chart) as Visual;
            RenderTargetBitmap bmpSource = new RenderTargetBitmap((int)chart.ActualWidth, (int)chart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            Rectangle backgroundRect = new Rectangle();
            backgroundRect.Fill = Brushes.White;
            backgroundRect.Arrange(new Rect(chart.RenderSize));
            bmpSource.Render(backgroundRect);
            bmpSource.Render(visual);
            MemoryStream outStream = new MemoryStream();
            {
                // from BitmapSource to System.Drawing.Bitmap 
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bmpSource));
                enc.Save(outStream);
            }
            ////Creating a Pdf document.
            PdfDocument pdfDoc = new PdfDocument();
            PdfPage page = pdfDoc.Pages.Add();
            //Getting a pdf image from Stream
            PdfImage img = PdfImage.FromStream(outStream);
            page.Graphics.DrawImage(img, new System.Drawing.PointF(10, 30));
            //Save the PDF Document to disk.
            pdfDoc.Save(pdfFile);
            System.Diagnostics.Process.Start(pdfFile);
            //Disposing the Stream;
            outStream.Close();
        }
    }
}
