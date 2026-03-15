using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Printing;
using WPFUtilities;
using System.Drawing.Printing;

namespace Utilities
{
    public class UtilitiesPrintHelper
    {
        public static bool PrintElement(FrameworkElement element, Window owner, bool fitToPage, bool landscape, PaperKind paperkind)
        {
            if (element == null)
                return true;
            try
            {
                var brush = new VisualBrush(element);
                var visual = new DrawingVisual();
                var context = visual.RenderOpen();

                context.DrawRectangle(brush, null,
                    new Rect(0, 0, element.ActualWidth, element.ActualHeight));
                context.Close();

                var bmp = new RenderTargetBitmap((int)element.ActualWidth,
                    (int)element.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                bmp.Render(visual);
                //var bitmapImage = new BitmapImage();
                var bitmapEncoder = new PngBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(bmp));

                using (var stream = new MemoryStream())
                {
                    bitmapEncoder.Save(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(stream);
                    using (XtraReport report = new XtraReport())
                    {
                        XRPictureBox PictureBox = new XRPictureBox();
                        PictureBox.Image = bitmap;
                        PictureBox.Sizing = DevExpress.XtraPrinting.ImageSizeMode.AutoSize;
                        PictureBox.WidthF = report.PageWidth - report.Margins.Left - report.Margins.Right;
                        PictureBox.HeightF = report.PageHeight - report.Margins.Top - report.Margins.Bottom;
                        ReportHeaderBand header = new ReportHeaderBand();
                        header.Controls.Add(PictureBox);
                        report.Bands.Add(header);
                        //var model = new XtraReportPreviewModel(report);
                        //model.ZoomMode = new ZoomFitModeItem(ZoomFitMode.WholePage);
                        DocumentPreviewWindow preview = new DocumentPreviewWindow() { /*Model = model, */Owner = owner };
                        preview.PreviewControl.DocumentSource = report;
                        ThemeHelper.SetTheme(preview);
                        report.Landscape = landscape;
                        report.PaperKind = paperkind;
                        report.CreateDocument(true);
                        if (fitToPage)
                            report.PrintingSystem.Document.AutoFitToPagesWidth = 1;
                        preview.ShowDialog();
                    }
                }
                return false;
            }
            catch
            {
                return true;
            }
        }
        public static bool PrintControl(Window owner, IPrintableControl source, string documentName, string title, bool landscape, PaperKind paperkind)
        {
            /// fitToPage not supported for IPrintableControl
            if (source == null)
                return true;
            try
            {
                using (var print = new PrintableControlLink(source))
                {
                    DocumentPreviewWindow preview = new DocumentPreviewWindow() { /*Model = model, */Owner = owner };
                    preview.PreviewControl.DocumentSource = print;
                    ThemeHelper.SetTheme(preview);
                    print.Landscape = landscape;
                    print.PaperKind = paperkind;
                    print.CreateDocument(true);
                    preview.ShowDialog();
                }
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}
