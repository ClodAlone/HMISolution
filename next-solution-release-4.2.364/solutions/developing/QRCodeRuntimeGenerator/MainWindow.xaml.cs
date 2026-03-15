using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Shape;
using DevExpress.XtraReports.UI;
using DevExpress.Utils;
using System.IO;
using System.Xml;
using System.Windows.Resources;
using Microsoft.Win32;
using Color = System.Windows.Media.Color;

namespace QRCodeRuntimeGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public String ImagePath;

        public MainWindow()
        {
            InitializeComponent();
            var t = DataContext as CommandLineOptions;
            if (t != null)
            {
                if (t.IsValid)
                {
                    QRCode.Text = t.QRCode;
                }
            }
        }

        private void previewbuttont_Click(object sender, RoutedEventArgs e)
        {
            PreviewReport();
        }

        private void printbuttont_Click(object sender, RoutedEventArgs e)
        {
            PrintReport();
        }
        #region #QRCode
        public XRBarCode CreateQRCodeBarCode(string BarCodeText)
        {
            // Create a bar code control.
            XRBarCode barCode = new XRBarCode();

            // Set the bar code's type to QRCode.
            barCode.Symbology = new QRCodeGenerator();

            // Adjust the bar code's main properties.
            barCode.Text = BarCodeText;
            barCode.Width = 400;
            barCode.Height = 150;

            // If the AutoModule property is set to false, uncomment the next line.
            barCode.AutoModule = true;
            // barcode.Module = 3;

            // Adjust the properties specific to the bar code type.
            ((QRCodeGenerator)barCode.Symbology).CompactionMode = QRCodeCompactionMode.Byte;
            ((QRCodeGenerator)barCode.Symbology).ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.H;
            ((QRCodeGenerator)barCode.Symbology).Version = QRCodeVersion.AutoVersion;

            return barCode;
        }
        private void CreateReportFooter(XtraReport report, string QRCode, string imagepath = null)
        {

            XRShape rectangle = new XRShape()
            {
                BackColor = System.Drawing.Color.Silver,
                BorderColor = System.Drawing.Color.Transparent,
                BorderWidth = 0,
                Shape = new ShapeRectangle(),
                Width = 629,
                Height = 16,
                Location = new System.Drawing.Point() { X = 10, Y = 4 }
            };
            XRLabel label = new XRLabel()
            {
                BackColor = System.Drawing.Color.White,
                ForeColor = System.Drawing.Color.CornflowerBlue,
                Width = 134,
                Height = 24,
                Location = new System.Drawing.Point() { X = 269, Y = 65 },
                Font = new Font("Segoe UI", 11, System.Drawing.FontStyle.Bold),
                Text = "QRCode value"
            };

            XRLabel vLabel = new XRLabel()
            {
                BackColor = System.Drawing.Color.White,
                ForeColor = System.Drawing.Color.Black,
                Width = 370,
                Height = 200,
                Location = new System.Drawing.Point() { X = 269, Y = 100 },
                Font = new Font("Segoe UI", 11, System.Drawing.FontStyle.Regular),
                Text = QRCode
            };

            // Create a bar code control.
            XRBarCode barCode = CreateQRCodeBarCode(QRCode);
            barCode.Width = barCode.Height = 244;
            barCode.Location = new System.Drawing.Point() { X = 10, Y = 65 };

            // Create a Detail band and add the bar code to it.
            report.Bands.Add(new DetailBand());
            report.Bands[BandKind.Detail].Controls.Add(rectangle);
            report.Bands[BandKind.Detail].Controls.Add(barCode);
            report.Bands[BandKind.Detail].Controls.Add(label);
            report.Bands[BandKind.Detail].Controls.Add(vLabel);

            if (!String.IsNullOrEmpty(imagepath))
            {
                XRPictureBox image = new XRPictureBox()
                {
                    ImageUrl = imagepath
                };
                barCode.Width = barCode.Height = 244;
                barCode.Location = new System.Drawing.Point() { X = 10 + 244 + 10, Y = 65 + 244 + 10 };
                report.Bands[BandKind.Detail].Controls.Add(image);
            }
        }

        private void CreateReportHeader(XtraReportBase report)
        {
            //Creating a Report header
            ReportHeaderBand header = new ReportHeaderBand();
            report.Bands.Add(header);
            header.HeightF = 0;

            XRLabel label = new XRLabel();
            header.Controls.Add(label);

            label.BackColor = System.Drawing.Color.White;
            label.ForeColor = System.Drawing.Color.DarkBlue;
            label.Font = new Font("Segoe UI", 24, System.Drawing.FontStyle.Bold);
            label.Text = "QRCode runtime generation";
            label.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            label.LocationF = new PointF(10, 0);

            XtraReport rep = report.RootReport;
            label.WidthF = rep.PageWidth - rep.Margins.Right - rep.Margins.Left;
        }

        #endregion #QRCode

        internal void PreviewReport()
        {
            // Create a report and preview it.
            XtraReport report = CreateReport();
            report.ShowPreviewDialog();
        }

        internal void PrintReport()
        {
            // Create a report and print it.
            XtraReport report = CreateReport();
            report.Print();
        }
        private XtraReport CreateReport()
        {
            XtraReport report = new XtraReport();

            CreateReportHeader(report);
            CreateReportFooter(report, QRCode.Text, ImagePath);

            return report;
        }
    }
}
