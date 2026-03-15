#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT
using System.Collections.Generic;
using System;


#if !XAML && !GDI
using System.Drawing;
using Syncfusion.Pdf.Graphics;
#elif XAML && !BARCODE_WINRT
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
#elif GDI
using System.Drawing;
#if WINFORMS
using System.Windows.Forms;
#endif
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;

#endif


# if WPF || BARCODE_SILVERLIGHT || BARCODE_WINRT
namespace Syncfusion.UI.Xaml.Controls.Barcode
#elif WINDOWS_PHONE
namespace Syncfusion.WP.Controls.Barcode
#elif ASPNET
namespace Syncfusion.Web.UI.WebControls.Barcode
#elif WINFORMS
namespace Syncfusion.Windows.Forms.Barcode
#elif MVC
namespace Syncfusion.Mvc.Barcode
#else
namespace Syncfusion.Pdf.Barcode
#endif
{
#if !XAML && !GDI
    public class PdfQRBarcode : PdfBidimensionalBarcode
#else
    public class QRBarcode : BidimensionalBarcode
#endif
    {
        #region Fields

        /// <summary>
        /// Holds the Version Information.
        /// </summary>
#if !XAML && !GDI
        private QRCodeVersion m_version = QRCodeVersion.Version01;
#else
        private QRBarcodeVersion m_version = QRBarcodeVersion.Version01;
#endif

        /// <summary>
        /// Holds the Number of Modules.
        /// </summary>
        private int m_noOfModules = 21;

        /// <summary>
        /// Holds the data of Function Pattern.
        /// </summary>
        private ModuleValue[,] m_moduleValue;

        /// <summary>
        /// Holds the Data in the Encoding Region.
        /// </summary>
        private ModuleValue[,] m_dataAllocationValues;

        /// <summary>
        /// Holds the Input Mode.
        /// </summary>
#if !XAML && !GDI
        private InputMode m_inputMode = InputMode.NumericMode;
#else
        private QRInputMode m_inputMode = QRInputMode.NumericMode;
#endif

        /// <summary>
        /// Holds the Error correction level.
        /// </summary>
#if !XAML && !GDI
        private PdfErrorCorrectionLevel m_errorCorrectionLevel = PdfErrorCorrectionLevel.Low;
#else
        private ErrorCorrectionLevel m_errorCorrectionLevel = ErrorCorrectionLevel.Low;
#endif

        /// <summary>
        /// Holds the Data Bit value.
        /// </summary>
        private int dataBits;

        /// <summary>
        /// Holds the Number of Blocks.
        /// </summary>
        private int[] blocks;

        /// <summary>
        /// Holds the Image.
        /// </summary>
#if !XAML && !NETFX_CORE && !WP
        private Bitmap image;
#endif

        /// <summary>
        /// Check if User Mentioned Mode.
        /// </summary>
        private bool m_IsUserMentionedMode = false;

        /// <summary>
        /// Check if User Mentioned Version.
        /// </summary>
        private bool m_IsUserMentionedVersion = false;

        /// <summary>
        /// Check if User Mentioned Error Correction Level.
        /// </summary>
        private bool m_IsUserMentionedErrorCorrectionLevel = false;

        /// <summary>
        /// Check if ECI.
        /// </summary>
        private bool m_IsEci = false;

        /// <summary>
        /// Holds the ECI Assignment Number.
        /// </summary>
        private int m_EciAssignmentNumber = 3;

        /// <summary>
        /// Variable to hold the QR Barcode Values.
        /// </summary>
        private PdfQRBarcodeValues m_qrBarcodeValues;

        #endregion

        #region Constructor
#if !XAML && !GDI
        public PdfQRBarcode()
#else
        public QRBarcode()
#endif
        {
            XDimension = 1;
            QuietZone.All = 2;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the QR Barcode Version.
        /// </summary>
#if !XAML && !GDI
        public QRCodeVersion Version
#else
        public QRBarcodeVersion QRVersion
#endif
        {
            get
            {
                return m_version;
            }
            set
            {
                m_version = value;
                m_noOfModules = ((int)m_version - 1) * 4 + 21;
#if !XAML && !GDI
                if (value != QRCodeVersion.Auto)
#else
                if (value != QRBarcodeVersion.Auto)
#endif
                    m_IsUserMentionedVersion = true;
            }
        }

        /// <summary>
        /// Gets or sets the Error correction level.
        /// </summary>
#if !XAML && !GDI
        public PdfErrorCorrectionLevel ErrorCorrectionLevel
#else
        public ErrorCorrectionLevel ErrorCorrectionLevel
#endif
        {
            get
            {
                return m_errorCorrectionLevel;
            }
            set
            {
                m_errorCorrectionLevel = value;
                m_IsUserMentionedErrorCorrectionLevel = true;
            }
        }

        /// <summary>
        /// Gets or sets the Mode of the input text.
        /// </summary>
#if !XAML && !GDI
        public InputMode InputMode
#else
        public QRInputMode InputMode
#endif
        {
            get
            {
                return m_inputMode;
            }
            set
            {
                m_inputMode = value;
                m_IsUserMentionedMode = true;
            }
        }

        #endregion

        #region Implementation
#if!XAML && !GDI
        /// <summary>
        /// Exports the barcode as image.
        /// <returns>The barcode image.</returns>
        /// </summary>
        /// <example>
        /// <code lang = "C#">
        /// //Creates a new PdfQRBarcode.
        /// PdfQRBarcode qrCode = new PdfQRBarcode();
        /// //Set the barcode text.
        /// qrCode.Text = "012345678";
        /// //Get the image for QR Barcode.
        /// Image image= qrCode.ToImage();
        /// //Save the image into Disk
        /// image.Save("QRCode.png", ImageFormat.Png);
        /// </code>
        /// <code lang="VB">
        /// 'Creates a new PdfQRBarcode.
        /// Dim qrCode As PdfQRBarcode = New PdfQRBarcode()
        /// 'Set the barcode text.
        /// qrCode.Text = "012345678"
        /// 'Get the image for Code32 Barcode.
        /// Image image= qrCode.ToImage()
        /// 'Save the image into Disk
        /// image.Save("QRCode.png", ImageFormat.Png)
        /// </code>
        /// </example>
#if !NETFX_CORE && !WP
        public override Image ToImage()
        {
            GenerateValues();

            PdfUnitConvertor convertor = new PdfUnitConvertor();
            int dimension = (int)convertor.ConvertToPixels(XDimension, PdfGraphicsUnit.Point);

            int quietZone = (int)QuietZone.All;

            int width = (m_noOfModules + 2 * quietZone) * dimension;
            int height = (m_noOfModules + 2 * quietZone) * dimension;

            int x = 0, y = 0;

            Bitmap bmp = new Bitmap((int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
            {
                Brush whiteBrush = Brushes.White;
                Brush blackBrush = Brushes.Black;

                int w = m_noOfModules + 2 * quietZone, h = m_noOfModules + 2 * quietZone;

                for (int i = 0; i < w; i++)
                {
                    x = 0;
                    for (int j = 0; j < h; j++)
                    {
                        Brush solidBrush = null;
                        if (m_moduleValue[i, j].IsBlack)
                            solidBrush = blackBrush;
                        else
                            solidBrush = whiteBrush;

                        if (m_dataAllocationValues[j, i].IsFilled)
                            if (m_dataAllocationValues[j, i].IsBlack)
                                solidBrush = blackBrush;

                        g.FillRectangle(solidBrush, new Rectangle(x, y, dimension, dimension));

                        x = x + dimension;
                    }

                    y = y + dimension;
                }
            }

            return bmp;
        }
#endif
        /// <summary>
        /// Draws the barcode on the <see cref="PdfPage"/> at the specified location.
        /// </summary>
        /// <param name="page">The pdf page.</param>
        /// <param name="location">The barcode location.</param>
        /// <example>
        /// <code lang = "C#">
        /// //Create a new PDF document.
        /// PdfDocument document = new PdfDocument();
        /// //Creates a new page and adds it as the last page of the document
        /// PdfPage page = document.Pages.Add();
        /// //Create font and font style.
        /// PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Bold) ;
        /// //Creates a new PdfQRBarcode.
        /// PdfQRBarcode qrCode = new PdfQRBarcode();
        /// //Set the barcode text.
        /// qrCode.Text = "012345678";
        /// //Draw a barcode in the new Page.
        /// qrCode.Draw(page, new PointF(25, 500));
        /// //Save the  document to disk.
        /// document.Save("QRBarcode.pdf");
        /// </code>
        /// <code lang="VB">
        /// 'Create a new PDF document.
        /// Dim document As PdfDocument = New PdfDocument()
        /// 'Creates a new page and adds it as the last page of the document
        /// Dim page As PdfPage = document.Pages.Add()
        /// 'Creates a new PdfQRBarcode.
        /// Dim qrCode As PdfQRBarcode = New PdfQRBarcode()
        /// 'Set the barcode text.
        /// qrCode.Text = "012345678"
        /// 'Draw a barcode in the new Page.
        /// qrCode.Draw(page, new PointF(25, 500))
        /// 'Save the  document to disk.
        /// document.Save("QRBarcode.pdf")
        /// </code>
        /// </example>
        public override void Draw(PdfPageBase page, PointF location)
        {
            GenerateValues();

            int quietZone = (int)QuietZone.All;

            // Draw in PdfPage.
            PdfBrush blackBrush = PdfBrushes.Black;
            PdfBrush whiteBrush = PdfBrushes.White;

            float x = location.X;
            float y = location.Y;

            int w = m_noOfModules + 2 * quietZone, h = m_noOfModules + 2 * quietZone;

            for (int i = 0; i < w; i++)
            {
                x = location.X;

                for (int j = 0; j < h; j++)
                {
                    PdfBrush colorBrush = null;
                    if (m_moduleValue[i, j].IsBlack)
                        colorBrush = blackBrush;
                    else
                        colorBrush = whiteBrush;

                    if (m_dataAllocationValues[j, i].IsFilled)
                        if (m_dataAllocationValues[j, i].IsBlack)
                            colorBrush = blackBrush;

                    page.Graphics.DrawRectangle(colorBrush, x, y, XDimension, XDimension);

                    x = x + XDimension;
                }

                y = y + XDimension;
            }
        }
#elif GDI
        public override Image Draw(int angle)
        {
            GenerateValues();

            int dimension = (int)XDimension;

            int quietZone = (int)QuietZone.All;

            int width = (m_noOfModules + 2 * quietZone) * dimension;
            int height = (m_noOfModules + 2 * quietZone) * dimension;

            int x = 0, y = 0;

            Bitmap bmp = null;
            if (angle == 90 || angle == 270)
                bmp = new Bitmap((int)height, (int)width, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            else
                bmp = new Bitmap((int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);


            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
            {
                System.Drawing.Drawing2D.GraphicsState state = g.Save();
                if (angle == 90 || angle == 270)
                {
                    g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                    g.RotateTransform(angle);
                    g.TranslateTransform(-(float)bmp.Height / 2, -(float)bmp.Width / 2);
                }

                Brush whiteBrush = Brushes.White;
                Brush blackBrush = Brushes.Black;

                int w = m_noOfModules + 2 * quietZone, h = m_noOfModules + 2 * quietZone;

                for (int i = 0; i < w; i++)
                {
                    x = 0;
                    for (int j = 0; j < h; j++)
                    {
                        Brush solidBrush = null;
                        if (m_moduleValue[i, j].IsBlack)
                            solidBrush = blackBrush;
                        else
                            solidBrush = whiteBrush;

                        if (m_dataAllocationValues[j, i].IsFilled)
                            if (m_dataAllocationValues[j, i].IsBlack)
                                solidBrush = blackBrush;

                        g.FillRectangle(solidBrush, new Rectangle(x, y, dimension, dimension));

                        x = x + dimension;
                    }

                    y = y + dimension;
                }

                if (state != null && (angle == 90 || angle == 270))
                    g.Restore(state);
            }

            return bmp;
        }

#if WINFORMS
        public SizeF Draw(Panel panel)
        {
            Graphics g = panel.CreateGraphics();
            g.Clear(Color.White);
            GenerateValues();

            int quietZone = (int)QuietZone.All;

            Brush blackBrush = Brushes.Black;
            Brush whiteBrush = Brushes.White;

            float x = 0;
            float y = 0;

            int w = m_noOfModules + 2 * quietZone, h = m_noOfModules + 2 * quietZone;

            for (int i = 0; i < w; i++)
            {
                x = 0;

                for (int j = 0; j < h; j++)
                {
                    Brush colorBrush = null;
                    if (m_moduleValue[i, j].IsBlack)
                        colorBrush = blackBrush;
                    else
                        colorBrush = whiteBrush;

                    if (m_dataAllocationValues[j, i].IsFilled)
                        if (m_dataAllocationValues[j, i].IsBlack)
                            colorBrush = blackBrush;

                    g.FillRectangle(colorBrush, x, y, XDimension, XDimension);

                    x = x + XDimension;
                }

                y = y + XDimension;
            }
            return new SizeF(x, y);
        }
#endif
#else
        internal override void Draw(Canvas m_canvas)
        {
            GenerateValues();

            Brush blackBrush = new SolidColorBrush(Colors.Black);
            Brush whiteBrush = new SolidColorBrush(Colors.White);

            float x = 0;
            float y = 0;

            int quietZone = (int)QuietZone.All;

            int w = m_noOfModules + 2 * quietZone, h = m_noOfModules + 2 * quietZone;

            for (int i = 0; i < w; i++)
            {
                x = 0;

                for (int j = 0; j < h; j++)
                {
                    Brush colorBrush = null;
                    if (m_moduleValue[i, j].IsBlack)
                        colorBrush = blackBrush;
                    else
                        colorBrush = whiteBrush;

                    if (m_dataAllocationValues[j, i].IsFilled)
                        if (m_dataAllocationValues[j, i].IsBlack)
                            colorBrush = blackBrush;

                    RectangleGeometry rectangleGeometry = new RectangleGeometry();

#if XAML && !BARCODE_WINRT
                    Path path = new Path();
#else
                    Windows.UI.Xaml.Shapes.Path path = new Windows.UI.Xaml.Shapes.Path();
#endif
                    path.Fill = colorBrush;
                    path.Stroke = colorBrush;
#if XAML && !BARCODE_WINRT
                    rectangleGeometry.Rect = new System.Windows.Rect(x, y, XDimension, XDimension);
#else
                    rectangleGeometry.Rect = new Rect(x, y, XDimension, XDimension);
#endif
                    path.Data = rectangleGeometry;

                    m_canvas.Children.Add(path);

                    x = x + XDimension;
                }

                y = y + XDimension;
            }

            m_canvas.Width = x;
            m_canvas.Height = y;
        }
#endif
        /// <summary>
        /// Generates the values of the QR Barcode
        /// </summary>
        private void GenerateValues()
        {
            Initialize();
            m_qrBarcodeValues = new PdfQRBarcodeValues(m_version, m_errorCorrectionLevel);



            m_moduleValue = new ModuleValue[m_noOfModules, m_noOfModules];


            DrawPDP(0, 0);
            DrawPDP(m_noOfModules - 7, 0);
            DrawPDP(0, m_noOfModules - 7);

            DrawTimingPattern();
#if !XAML && !GDI
            if (m_version != QRCodeVersion.Version01)
#else
            if (m_version != QRBarcodeVersion.Version01)
#endif
            {
                int[] allignCoOrdinates = GetAlignmentPatternCoOrdinates();

                foreach (int i in allignCoOrdinates)
                    foreach (int j in allignCoOrdinates)
                    {
                        if (m_moduleValue[i, j].IsPDP != true)
                        {
                            DrawAlignmentPattern(i, j);
                        }

                    }
            }
            AllocateFormatAndVersionInformation();

            bool[] encodeData = EncodeData();

            DataAllocationAndMasking(encodeData);

            DrawFormatInformation();
            AddQuietZone();
        }

        /// <summary>
        /// Adds quietzone to the QR Barcode.
        /// </summary>
        private void AddQuietZone()
        {
            int quietZone = (int)QuietZone.All;
            int w = m_noOfModules + 2 * quietZone;
            int h = m_noOfModules + 2 * quietZone;

            ModuleValue[,] tempValue1 = new ModuleValue[w, h];
            ModuleValue[,] tempValue2 = new ModuleValue[w, h];

            // Top quietzone.
            for (int i = 0; i < h; i++)
            {
                tempValue1[0, i] = new ModuleValue();
                tempValue1[0, i].IsBlack = false;
                tempValue1[0, i].IsFilled = false;
                tempValue1[0, i].IsPDP = false;
                tempValue2[0, i] = new ModuleValue();
                tempValue2[0, i].IsBlack = false;
                tempValue2[0, i].IsFilled = false;
                tempValue2[0, i].IsPDP = false;
            }

            for (int i = quietZone; i < w - quietZone; i++)
            {
                // Left quietzone.
                tempValue1[i, 0] = new ModuleValue();
                tempValue1[i, 0].IsBlack = false;
                tempValue1[i, 0].IsFilled = false;
                tempValue1[i, 0].IsPDP = false;
                tempValue2[i, 0] = new ModuleValue();
                tempValue2[i, 0].IsBlack = false;
                tempValue2[i, 0].IsFilled = false;
                tempValue2[i, 0].IsPDP = false;

                for (int j = quietZone; j < h - quietZone; j++)
                {
                    tempValue1[i, j] = m_moduleValue[i - quietZone, j - quietZone];
                    tempValue2[i, j] = m_dataAllocationValues[i - quietZone, j - quietZone];
                }

                // Right quietzone.
                tempValue1[i, h - quietZone] = new ModuleValue();
                tempValue1[i, h - quietZone].IsBlack = false;
                tempValue1[i, h - quietZone].IsFilled = false;
                tempValue1[i, h - quietZone].IsPDP = false;
                tempValue2[i, h - quietZone] = new ModuleValue();
                tempValue2[i, h - quietZone].IsBlack = false;
                tempValue2[i, h - quietZone].IsFilled = false;
                tempValue2[i, h - quietZone].IsPDP = false;
            }

            //Bottom quietzone.
            for (int i = 0; i < h; i++)
            {
                tempValue1[w - quietZone, i] = new ModuleValue();
                tempValue1[w - quietZone, i].IsBlack = false;
                tempValue1[w - quietZone, i].IsFilled = false;
                tempValue1[w - quietZone, i].IsPDP = false;
                tempValue2[w - quietZone, i] = new ModuleValue();
                tempValue2[w - quietZone, i].IsBlack = false;
                tempValue2[w - quietZone, i].IsFilled = false;
                tempValue2[w - quietZone, i].IsPDP = false;
            }

            m_moduleValue = tempValue1;
            m_dataAllocationValues = tempValue2;
        }

        /// <summary>
        /// Draw the PDP in the given location
        /// </summary>
        /// <param name="x">The x co-ordinate.</param>
        /// <param name="y">The y co-ordinate.</param>
        private void DrawPDP(int x, int y)
        {
            int i, j;
            for (i = x, j = y; i < x + 7; i++, j++)
            {
                m_moduleValue[i, y].IsBlack = true;
                m_moduleValue[i, y].IsFilled = true;
                m_moduleValue[i, y].IsPDP = true;

                m_moduleValue[i, y + 6].IsBlack = true;
                m_moduleValue[i, y + 6].IsFilled = true;
                m_moduleValue[i, y + 6].IsPDP = true;

                if (y + 7 < m_noOfModules)
                {
                    m_moduleValue[i, y + 7].IsBlack = false;
                    m_moduleValue[i, y + 7].IsFilled = true;
                    m_moduleValue[i, y + 7].IsPDP = true;
                }
                else if (y - 1 >= 0)
                {
                    m_moduleValue[i, y - 1].IsBlack = false;
                    m_moduleValue[i, y - 1].IsFilled = true;
                    m_moduleValue[i, y - 1].IsPDP = true;
                }

                m_moduleValue[x, j].IsBlack = true;
                m_moduleValue[x, j].IsFilled = true;
                m_moduleValue[x, j].IsPDP = true;

                m_moduleValue[x + 6, j].IsBlack = true;
                m_moduleValue[x + 6, j].IsFilled = true;
                m_moduleValue[x + 6, j].IsPDP = true;

                if (x + 7 < m_noOfModules)
                {
                    m_moduleValue[x + 7, j].IsBlack = false;
                    m_moduleValue[x + 7, j].IsFilled = true;
                    m_moduleValue[x + 7, j].IsPDP = true;
                }
                else if (x - 1 >= 0)
                {
                    m_moduleValue[x - 1, j].IsBlack = false;
                    m_moduleValue[x - 1, j].IsFilled = true;
                    m_moduleValue[x - 1, j].IsPDP = true;
                }

            }

            if (x + 7 < m_noOfModules && y + 7 < m_noOfModules)
            {
                m_moduleValue[x + 7, y + 7].IsBlack = false;
                m_moduleValue[x + 7, y + 7].IsFilled = true;
                m_moduleValue[x + 7, y + 7].IsPDP = true;
            }
            else if (x + 7 < m_noOfModules && y + 7 >= m_noOfModules)
            {
                m_moduleValue[x + 7, y - 1].IsBlack = false;
                m_moduleValue[x + 7, y - 1].IsFilled = true;
                m_moduleValue[x + 7, y - 1].IsPDP = true;
            }
            else if (x + 7 >= m_noOfModules && y + 7 < m_noOfModules)
            {
                m_moduleValue[x - 1, y + 7].IsBlack = false;
                m_moduleValue[x - 1, y + 7].IsFilled = true;
                m_moduleValue[x - 1, y + 7].IsPDP = true;
            }

            x++;
            y++;
            for (i = x, j = y; i < x + 5; i++, j++)
            {
                m_moduleValue[i, y].IsBlack = false;
                m_moduleValue[i, y].IsFilled = true;
                m_moduleValue[i, y].IsPDP = true;

                m_moduleValue[i, y + 4].IsBlack = false;
                m_moduleValue[i, y + 4].IsFilled = true;
                m_moduleValue[i, y + 4].IsPDP = true;

                m_moduleValue[x, j].IsBlack = false;
                m_moduleValue[x, j].IsFilled = true;
                m_moduleValue[x, j].IsPDP = true;

                m_moduleValue[x + 4, j].IsBlack = false;
                m_moduleValue[x + 4, j].IsFilled = true;
                m_moduleValue[x + 4, j].IsPDP = true;
            }

            x++;
            y++;
            for (i = x, j = y; i < x + 3; i++, j++)
            {
                m_moduleValue[i, y].IsBlack = true;
                m_moduleValue[i, y].IsFilled = true;
                m_moduleValue[i, y].IsPDP = true;

                m_moduleValue[i, y + 2].IsBlack = true;
                m_moduleValue[i, y + 2].IsFilled = true;
                m_moduleValue[i, y + 2].IsPDP = true;

                m_moduleValue[x, j].IsBlack = true;
                m_moduleValue[x, j].IsFilled = true;
                m_moduleValue[x, j].IsPDP = true;

                m_moduleValue[x + 2, j].IsBlack = true;
                m_moduleValue[x + 2, j].IsFilled = true;
                m_moduleValue[x + 2, j].IsPDP = true;
            }
            m_moduleValue[x + 1, y + 1].IsBlack = true;
            m_moduleValue[x + 1, y + 1].IsFilled = true;
            m_moduleValue[x + 1, y + 1].IsPDP = true;


        }

        /// <summary>
        /// Draw the Timing Pattern
        /// </summary>
        private void DrawTimingPattern()
        {
            for (int i = 8; i < m_noOfModules - 8; i += 2)
            {
                m_moduleValue[i, 6].IsBlack = true;
                m_moduleValue[i, 6].IsFilled = true;

                m_moduleValue[i + 1, 6].IsBlack = false;
                m_moduleValue[i + 1, 6].IsFilled = true;

                m_moduleValue[6, i].IsBlack = true;
                m_moduleValue[6, i].IsFilled = true;

                m_moduleValue[6, i + 1].IsBlack = false;
                m_moduleValue[6, i + 1].IsFilled = true;

            }
            m_moduleValue[m_noOfModules - 8, 8].IsBlack = true;
            m_moduleValue[m_noOfModules - 8, 8].IsFilled = true;
        }

        /// <summary>
        /// Draw the Alignment Pattern in the given location
        /// </summary>
        /// <param name="x">The x co-ordinate.</param>
        /// <param name="y">The y co-ordinate.</param>
        private void DrawAlignmentPattern(int x, int y)
        {
            int i, j;
            for (i = x - 2, j = y - 2; i < x + 3; i++, j++)
            {
                m_moduleValue[i, y - 2].IsBlack = true;
                m_moduleValue[i, y - 2].IsFilled = true;

                m_moduleValue[i, y + 2].IsBlack = true;
                m_moduleValue[i, y + 2].IsFilled = true;

                m_moduleValue[x - 2, j].IsBlack = true;
                m_moduleValue[x - 2, j].IsFilled = true;

                m_moduleValue[x + 2, j].IsBlack = true;
                m_moduleValue[x + 2, j].IsFilled = true;
            }

            for (i = x - 1, j = y - 1; i < x + 2; i++, j++)
            {
                m_moduleValue[i, y - 1].IsBlack = false;
                m_moduleValue[i, y - 1].IsFilled = true;

                m_moduleValue[i, y + 1].IsBlack = false;
                m_moduleValue[i, y + 1].IsFilled = true;

                m_moduleValue[x - 1, j].IsBlack = false;
                m_moduleValue[x - 1, j].IsFilled = true;

                m_moduleValue[x + 1, j].IsBlack = false;
                m_moduleValue[x + 1, j].IsFilled = true;
            }
            m_moduleValue[x, y].IsBlack = true;
            m_moduleValue[x, y].IsFilled = true;
        }

        /// <summary>
        /// Encode the Input Data
        /// </summary>
        private bool[] EncodeData()
        {
            List<bool> encodeData = new List<bool>();

            #region Add Mode Indicator

            switch (m_inputMode)
            {
#if !XAML && !GDI
                case InputMode.NumericMode:
#else
                case QRInputMode.NumericMode:
#endif
                    encodeData.Add(false);
                    encodeData.Add(false);
                    encodeData.Add(false);
                    encodeData.Add(true);
                    break;
#if !XAML && !GDI
                case InputMode.AlphaNumericMode:
#else
                case QRInputMode.AlphaNumericMode:
#endif
                    encodeData.Add(false);
                    encodeData.Add(false);
                    encodeData.Add(true);
                    encodeData.Add(false);
                    break;
#if !XAML && !GDI
                case InputMode.BinaryMode:
#else
                case QRInputMode.BinaryMode:
#endif
                    if (m_IsEci)
                    {
                        //Add ECI Mode Indicator
                        encodeData.Add(false);
                        encodeData.Add(true);
                        encodeData.Add(true);
                        encodeData.Add(true);

                        //Add ECI assignment number
                        bool[] numberInBool = StringToBoolArray(m_EciAssignmentNumber.ToString(), 8);
                        foreach (bool x in numberInBool)
                            encodeData.Add(x);
                    }
                    encodeData.Add(false);
                    encodeData.Add(true);
                    encodeData.Add(false);
                    encodeData.Add(false);
                    break;

                //#if !BARCODE_WINRT
                //                //case InputMode.KanjiMode:
                //#else

                //#endif
                //    encodeData.Add(true);
                //    encodeData.Add(false);
                //    encodeData.Add(false);
                //    encodeData.Add(false);
                //    break;
            }
            #endregion

            #region Add Character count Indicator

            int numberOfBitsInCharacterCountIndicator = 0;
            if ((int)m_version < 10)
            {
                switch (m_inputMode)
                {
#if !XAML && !GDI
                    case InputMode.NumericMode:
#else
                    case QRInputMode.NumericMode:
#endif
                        numberOfBitsInCharacterCountIndicator = 10;
                        break;
#if !XAML && !GDI
                    case InputMode.AlphaNumericMode:
#else
                    case QRInputMode.AlphaNumericMode:
#endif
                        numberOfBitsInCharacterCountIndicator = 9;
                        break;
#if !XAML && !GDI
                    case InputMode.BinaryMode:
#else
                    case QRInputMode.BinaryMode:
#endif
                        numberOfBitsInCharacterCountIndicator = 8;
                        break;

                    //case InputMode.KanjiMode:

                }
            }
            else if ((int)m_version < 27)
            {
                switch (m_inputMode)
                {
#if !XAML && !GDI
                    case InputMode.NumericMode:
#else
                    case QRInputMode.NumericMode:
#endif
                        numberOfBitsInCharacterCountIndicator = 12;
                        break;
#if !XAML && !GDI
                    case InputMode.AlphaNumericMode:
#else
                    case QRInputMode.AlphaNumericMode:
#endif
                        numberOfBitsInCharacterCountIndicator = 11;
                        break;
#if !XAML && !GDI
                    case InputMode.BinaryMode:
#else
                    case QRInputMode.BinaryMode:
#endif
                        numberOfBitsInCharacterCountIndicator = 16;
                        break;
                    //case InputMode.KanjiMode:
                    //    numberOfBitsInCharacterCountIndicator = 10;
                    //    break;
                }
            }
            else
            {
                switch (m_inputMode)
                {
#if !XAML && !GDI
                    case InputMode.NumericMode:
#else
                    case QRInputMode.NumericMode:
#endif
                        numberOfBitsInCharacterCountIndicator = 14;
                        break;
#if !XAML && !GDI
                    case InputMode.AlphaNumericMode:
#else
                    case QRInputMode.AlphaNumericMode:
#endif
                        numberOfBitsInCharacterCountIndicator = 13;
                        break;
#if !XAML &&!GDI
                    case InputMode.BinaryMode:
#else
                    case QRInputMode.BinaryMode:
#endif
                        numberOfBitsInCharacterCountIndicator = 16;
                        break;
                    //case InputMode.KanjiMode:
                    //    numberOfBitsInCharacterCountIndicator = 12;
                    //    break;
                }
            }

            bool[] numberOfBitsInCharacterCountIndicatorInBool = IntToBoolArray(Text.Length, numberOfBitsInCharacterCountIndicator);

            for (int i = 0; i < numberOfBitsInCharacterCountIndicator; i++)
            {
                encodeData.Add(numberOfBitsInCharacterCountIndicatorInBool[i]);
            }
            #endregion

            #region Encoding Data in Binary representation

#if !XAML && !GDI
            if (m_inputMode == InputMode.NumericMode)
#else
            if (m_inputMode == QRInputMode.NumericMode)
#endif
            {
                char[] dataStringArray = Text.ToCharArray();
                string number = "";
                for (int i = 0; i < dataStringArray.Length; i++)
                {
                    bool[] numberInBool;
                    number += dataStringArray[i];

                    if (i % 3 == 2 && i != 0 || i == dataStringArray.Length - 1)
                    {
                        if (number.ToString().Length == 3)
                            numberInBool = StringToBoolArray(number, 10);
                        else if (number.ToString().Length == 2)
                            numberInBool = StringToBoolArray(number, 7);
                        else
                            numberInBool = StringToBoolArray(number, 4);
                        number = "";
                        foreach (bool x in numberInBool)
                            encodeData.Add(x);
                    }
                }
            }
#if !XAML && !GDI
            else if (m_inputMode == InputMode.AlphaNumericMode)
#else
            else if (m_inputMode == QRInputMode.AlphaNumericMode)
#endif
            {
                char[] dataStringArray = Text.ToCharArray();
                string numberInString = "";
                int number = 0;
                for (int i = 0; i < dataStringArray.Length; i++)
                {
                    bool[] numberInBool;
                    numberInString += dataStringArray[i];

                    if (i % 2 == 0 && i + 1 != dataStringArray.Length)
                        number = 45 * m_qrBarcodeValues.GetAlphanumericvalues(dataStringArray[i]);

                    if (i % 2 == 1 && i != 0)
                    {
                        number += m_qrBarcodeValues.GetAlphanumericvalues(dataStringArray[i]);
                        numberInBool = IntToBoolArray(number, 11);
                        number = 0;
                        foreach (bool x in numberInBool)
                            encodeData.Add(x);
                        numberInString = null;
                    }
                    if (i != 1 && numberInString != null)
                        if (i + 1 == dataStringArray.Length && numberInString.Length == 1)
                        {
                            number = m_qrBarcodeValues.GetAlphanumericvalues(dataStringArray[i]);
                            numberInBool = IntToBoolArray(number, 6);
                            number = 0;
                            foreach (bool x in numberInBool)
                                encodeData.Add(x);
                        }
                }
            }

#if !XAML && !GDI
            else if (m_inputMode == InputMode.BinaryMode)
#else
            else if (m_inputMode == QRInputMode.BinaryMode)
#endif
            {
                char[] dataStringArray = Text.ToCharArray();
                for (int i = 0; i < dataStringArray.Length; i++)
                {
                    int number = 0;
                    if (((int)Text[i] >= 32 && (int)Text[i] <= 126) || ((int)Text[i] >= 161 && (int)Text[i] <= 255))
                    {
                        number = (int)dataStringArray[i];
                    }
                    else if ((int)Text[i] >= 65377 && (int)Text[i] <= 65439)
                    {
                        number = (int)dataStringArray[i] - 65216;
                    }
                    else
                    {
#if !XAML && !GDI && !NETFX_CORE && !WP
                        throw new PdfBarcodeException("Input text contains non-convertable characters");
#else
                        throw new BarcodeException("Input text contains non-convertable characters");
#endif
                    }

                    bool[] numberInBool = IntToBoolArray(number, 8);
                    foreach (bool x in numberInBool)
                        encodeData.Add(x);
                }
            }
            #endregion

            #region Adding Terminator

            for (int i = 0; i < 4; i++)
                if ((int)encodeData.Count / 8 == m_qrBarcodeValues.NumberOfDataCodeWord)
                    break;
                else
                    encodeData.Add(false);
            #endregion

            #region Encode to Code words

            for (; ; )              //Add Padding Bits
            {
                if (encodeData.Count % 8 == 0)
                    break;
                else
                    encodeData.Add(false);
            }

            for (; ; )
            {
                if (encodeData.Count / 8 == m_qrBarcodeValues.NumberOfDataCodeWord)
                    break;
                else
                {
                    encodeData.Add(true);       //11101100
                    encodeData.Add(true);
                    encodeData.Add(true);
                    encodeData.Add(false);
                    encodeData.Add(true);
                    encodeData.Add(true);
                    encodeData.Add(false);
                    encodeData.Add(false);
                }
                if (encodeData.Count / 8 == m_qrBarcodeValues.NumberOfDataCodeWord)
                    break;
                else
                {
                    encodeData.Add(false);       //00010001
                    encodeData.Add(false);
                    encodeData.Add(false);
                    encodeData.Add(true);
                    encodeData.Add(false);
                    encodeData.Add(false);
                    encodeData.Add(false);
                    encodeData.Add(true);
                }
            }

            dataBits = m_qrBarcodeValues.NumberOfDataCodeWord;
            blocks = m_qrBarcodeValues.NumberOfErrorCorrectionBlocks;

            int totalBlockSize = blocks[0];
            if (blocks.Length == 6)
                totalBlockSize = blocks[0] + blocks[3];

            string[][] ds1 = new string[totalBlockSize][];

            List<bool> testEncodeData = encodeData;
            if (blocks.Length == 6)     //If seperated into two seperate blocks
            {
                int dataCodeWordLength = blocks[0] * blocks[2] * 8;
                testEncodeData = new List<bool>();
                for (int i = 0; i < dataCodeWordLength; i++)
                {
                    testEncodeData.Add(encodeData[i]);
                }
            }

            string[,] dsOne = new string[blocks[0], testEncodeData.Count / 8 / blocks[0]];
            dsOne = CreateBlocks(testEncodeData, blocks[0]);

            for (int i = 0; i < blocks[0]; i++)
            {
                ds1[i] = SplitCodeWord(dsOne, i, testEncodeData.Count / 8 / blocks[0]);
            }

            if (blocks.Length == 6)
            {
                testEncodeData = new List<bool>();
                for (int i = blocks[0] * blocks[2] * 8; i < encodeData.Count; i++)
                {
                    testEncodeData.Add(encodeData[i]);
                }

                string[,] dsTwo = new string[blocks[0], testEncodeData.Count / 8 / blocks[3]];
                dsTwo = CreateBlocks(testEncodeData, blocks[3]);

                for (int i = blocks[0], count = 0; i < totalBlockSize; i++)
                {
                    ds1[i] = SplitCodeWord(dsTwo, count++, testEncodeData.Count / 8 / blocks[3]);
                }
            }
            encodeData = null;
            encodeData = new List<bool>();
            for (int i = 0; i < 125; i++)
            {
                for (int k = 0; k < totalBlockSize; k++)
                    for (int j = 0; j < 8; j++)
                        if (i < ds1[k].Length)
                            encodeData.Add(ds1[k][i][j] == '1' ? true : false);
            }

            #endregion

            #region Calculating Error correcting Code words
            {
#if!XAML && !GDI
                PdfErrorCorrectionCodewords ec = new PdfErrorCorrectionCodewords(m_version, m_errorCorrectionLevel);
#else
                ErrorCorrectionCodewords ec = new ErrorCorrectionCodewords(m_version, m_errorCorrectionLevel);
#endif
                dataBits = m_qrBarcodeValues.NumberOfDataCodeWord;
                int eccw = m_qrBarcodeValues.NumberOfErrorCorrectingCodeWords;
                blocks = m_qrBarcodeValues.NumberOfErrorCorrectionBlocks;


                if (blocks.Length == 6)
                    ec.DataBits = (dataBits - blocks[3] * blocks[5]) / blocks[0];
                else
                    ec.DataBits = dataBits / blocks[0];
                ec.ECCW = eccw / totalBlockSize;

                string[][] polynomial = new string[totalBlockSize][];
                int count = 0;

                for (int i = 0; i < blocks[0]; i++)
                {
                    ec.DC = ds1[count];
                    polynomial[count++] = ec.GetERCW();
                }
                if (blocks.Length == 6)
                {
                    ec.DataBits = (dataBits - blocks[0] * blocks[2]) / blocks[3];

                    for (int i = 0; i < blocks[3]; i++)
                    {
                        ec.DC = ds1[count];
                        polynomial[count++] = ec.GetERCW();
                    }
                }
                if (blocks.Length != 6)
                    for (int i = 0; i < polynomial[0].Length; i++)
                    {
                        for (int k = 0; k < blocks[0]; k++)
                            for (int j = 0; j < 8; j++)
                                if (i < polynomial[k].Length)
                                    encodeData.Add(polynomial[k][i][j] == '1' ? true : false);
                    }
                else
                    for (int i = 0; i < polynomial[0].Length; i++)
                    {
                        for (int k = 0; k < totalBlockSize; k++)
                            for (int j = 0; j < 8; j++)
                                if (i < polynomial[k].Length)
                                    encodeData.Add(polynomial[k][i][j] == '1' ? true : false);
                    }

            }
            #endregion

            return encodeData.ToArray();
        }

        /// <summary>
        /// Allocates the Encoded Data and then Mask
        /// </summary>
        /// <param name="Data">Encoded Data.</param>
        private void DataAllocationAndMasking(bool[] Data)
        {
            m_dataAllocationValues = new ModuleValue[m_noOfModules, m_noOfModules];
            int point = 0;

            for (int i = m_noOfModules - 1; i >= 0; i -= 2)
            {
                #region Bottom to Top

                for (int j = m_noOfModules - 1; j >= 0; j--)
                {
                    if (!(m_moduleValue[i, j].IsFilled && m_moduleValue[i - 1, j].IsFilled))
                    {
                        if (!m_moduleValue[i, j].IsFilled)
                        {
                            if (point + 1 < Data.Length)
                                m_dataAllocationValues[i, j].IsBlack = Data[point++];
                            if ((i + j) % 3 == 0)
                            {
                                if (m_dataAllocationValues[i, j].IsBlack)
                                    m_dataAllocationValues[i, j].IsBlack = true;
                                else
                                    m_dataAllocationValues[i, j].IsBlack = false;

                            }
                            else
                            {
                                if (m_dataAllocationValues[i, j].IsBlack)
                                    m_dataAllocationValues[i, j].IsBlack = false;
                                else
                                    m_dataAllocationValues[i, j].IsBlack = true;
                            }

                            m_dataAllocationValues[i, j].IsFilled = true;
                        }

                        if (!m_moduleValue[i - 1, j].IsFilled)
                        {
                            if (point + 1 < Data.Length)
                                m_dataAllocationValues[i - 1, j].IsBlack = Data[point++];
                            if ((i - 1 + j) % 3 == 0)
                            {
                                if (m_dataAllocationValues[i - 1, j].IsBlack)
                                    m_dataAllocationValues[i - 1, j].IsBlack = true;
                                else
                                    m_dataAllocationValues[i - 1, j].IsBlack = false;

                            }
                            else
                            {
                                if (m_dataAllocationValues[i - 1, j].IsBlack)
                                    m_dataAllocationValues[i - 1, j].IsBlack = false;
                                else
                                    m_dataAllocationValues[i - 1, j].IsBlack = true;
                            }
                            m_dataAllocationValues[i - 1, j].IsFilled = true;
                        }
                    }



                }
                #endregion

                i -= 2;
                if (i == 6)
                    i--;

                #region Top to Bottom

                for (int j = 0; j < m_noOfModules; j++)
                {
                    if (!(m_moduleValue[i, j].IsFilled && m_moduleValue[i - 1, j].IsFilled))
                    {
                        if (!m_moduleValue[i, j].IsFilled)
                        {
                            if (point + 1 < Data.Length)
                                m_dataAllocationValues[i, j].IsBlack = Data[point++];
                            if ((i + j) % 3 == 0)
                            {
                                if (m_dataAllocationValues[i, j].IsBlack)
                                    m_dataAllocationValues[i, j].IsBlack = true;
                                else
                                    m_dataAllocationValues[i, j].IsBlack = false;

                            }
                            else
                            {
                                if (m_dataAllocationValues[i, j].IsBlack)
                                    m_dataAllocationValues[i, j].IsBlack = false;
                                else
                                    m_dataAllocationValues[i, j].IsBlack = true;
                            }
                            m_dataAllocationValues[i, j].IsFilled = true;
                        }

                        if (!m_moduleValue[i - 1, j].IsFilled)
                        {
                            if (point + 1 < Data.Length)
                                m_dataAllocationValues[i - 1, j].IsBlack = Data[point++];
                            if ((i - 1 + j) % 3 == 0)
                            {
                                if (m_dataAllocationValues[i - 1, j].IsBlack)
                                    m_dataAllocationValues[i - 1, j].IsBlack = true;
                                else
                                    m_dataAllocationValues[i - 1, j].IsBlack = false;

                            }
                            else
                            {
                                if (m_dataAllocationValues[i - 1, j].IsBlack)
                                    m_dataAllocationValues[i - 1, j].IsBlack = false;
                                else
                                    m_dataAllocationValues[i - 1, j].IsBlack = true;
                            }
                            m_dataAllocationValues[i - 1, j].IsFilled = true;
                        }
                    }



                }
                #endregion
            }
            for (int i = 0; i < m_noOfModules; i++)
                for (int j = 0; j < m_noOfModules; j++)
                    if (!m_moduleValue[i, j].IsFilled)
                    {
                        bool flag = m_dataAllocationValues[i, j].IsBlack;
                        if (flag)
                            m_dataAllocationValues[i, j].IsBlack = false;
                        else
                            m_dataAllocationValues[i, j].IsBlack = true;
                    }


        }

        /// <summary>
        /// Draw the Format Information
        /// </summary>
        private void DrawFormatInformation()
        {
            byte[] formatInformation = m_qrBarcodeValues.FormatInformation;

            int count = 0;
            for (int i = 0; i < 7; i++)
            {
                //Draw from 14 to 8
                if (i == 6)
                    m_moduleValue[i + 1, 8].IsBlack = formatInformation[count] == 1 ? true : false;
                else
                    m_moduleValue[i, 8].IsBlack = formatInformation[count] == 1 ? true : false;

                m_moduleValue[8, m_noOfModules - i - 1].IsBlack = formatInformation[count++] == 1 ? true : false;


            }
            count = 14;
            for (int i = 0; i < 7; i++)
            {
                //Draw from 0 to 6
                if (i == 6)
                    m_moduleValue[8, i + 1].IsBlack = formatInformation[count] == 1 ? true : false;
                else
                    m_moduleValue[8, i].IsBlack = formatInformation[count] == 1 ? true : false;
                m_moduleValue[m_noOfModules - i - 1, 8].IsBlack = formatInformation[count--] == 1 ? true : false;
            }

            //Draw 7
            m_moduleValue[8, 8].IsBlack = formatInformation[7] == 1 ? true : false;
            m_moduleValue[8, m_noOfModules - 8].IsBlack = formatInformation[7] == 1 ? true : false;

        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Initializes the Version, Error correction level, Input Mode.
        /// </summary>
        private void Initialize()
        {
            #region Initialize Input Mode
#if !XAML && !GDI
            InputMode mode = InputMode.NumericMode;
#else
            QRInputMode mode = QRInputMode.NumericMode;
#endif
            for (int i = 0; i < Text.Length; i++)
            {
                if ((int)Text[i] < 58 && (int)Text[i] > 47)
                {
                    //numeric only
                }
                else if (((int)Text[i] < 91 && (int)Text[i] > 64) || Text[i] == '$' || Text[i] == '%' || Text[i] == '*' || Text[i] == '+' || Text[i] == '-' || Text[i] == '.' || Text[i] == '/' || Text[i] == ':' || Text[i] == ' ')
                {
#if !XAML && !GDI
                    mode = InputMode.AlphaNumericMode;
#else
                    mode = QRInputMode.AlphaNumericMode;
#endif
                }
                else if ((int)Text[i] >= 65377 && (int)Text[i] <= 65439)
                {
#if !XAML && !GDI
                    mode = InputMode.BinaryMode;
#else
                    mode = QRInputMode.BinaryMode;
#endif
                    break;
                }
                else
                {
#if !XAML && !GDI
                    mode = InputMode.BinaryMode;
#else
                    mode = QRInputMode.BinaryMode;
#endif
                    m_IsEci = true;
                    break;
                }
            }
            if (m_IsUserMentionedMode)
#if !XAML && !GDI
#if !NETFX_CORE && !WP
                if (mode != m_inputMode)
                    if (((mode == InputMode.AlphaNumericMode || mode == InputMode.BinaryMode) && m_inputMode == InputMode.NumericMode) || (mode == InputMode.BinaryMode && m_inputMode == InputMode.AlphaNumericMode))
                        throw new PdfBarcodeException("Mode Conflict");
#else
                if (mode != m_inputMode)
                    if (((mode == InputMode.AlphaNumericMode || mode == InputMode.BinaryMode) && m_inputMode == InputMode.NumericMode) || (mode == InputMode.BinaryMode && m_inputMode == InputMode.AlphaNumericMode))
                        throw new BarcodeException("Mode Conflict");
#endif

#else
                if (mode != m_inputMode)
                    if (((mode == QRInputMode.AlphaNumericMode || mode == QRInputMode.BinaryMode) && m_inputMode == QRInputMode.NumericMode) || (mode == QRInputMode.BinaryMode && m_inputMode == QRInputMode.AlphaNumericMode))
                        throw new BarcodeException("Mode Conflict");
#endif
            InputMode = mode;
            #endregion

            #region Initialize ECI Mode
            if (m_IsEci == true)
            {
                for (int i = 0; i < Text.Length; i++)
                {
                    if ((int)Text[i] >= 32 && (int)Text[i] <= 255)
                    {
                        continue;
                    }
                    //Check for CP437
                    if (IsCP437Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 2;
                        break;
                    }

                    //Check for ISO/IEC 8859-2
                    else if (IsISO8859_2Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 4;
                        break;
                    }
                    //Check for ISO/IEC 8859-3
                    else if (IsISO8859_3Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 5;
                        break;
                    }
                    //Check for ISO/IEC 8859-4
                    else if (IsISO8859_4Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 6;
                        break;
                    }
                    //Check for ISO/IEC 8859-5
                    else if (IsISO8859_5Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 7;
                        break;
                    }
                    //Check for ISO/IEC 8859-6
                    else if (IsISO8859_6Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 8;
                        break;
                    }
                    //Check for ISO/IEC 8859-7
                    else if (IsISO8859_7Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 9;
                        break;
                    }
                    //Check for ISO/IEC 8859-8
                    else if (IsISO8859_8Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 10;
                        break;
                    }
                    //Check for ISO/IEC 8859-8
                    else if (IsISO8859_11Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 13;
                        break;
                    }

                    //Check for Windows1250
                    else if (IsWindows1250Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 21;
                        break;
                    }
                    //Check for Windows1251
                    else if (IsWindows1251Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 22;
                        break;
                    }
                    //Check for Windows1252
                    else if (IsWindows1252Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 23;
                        break;
                    }
                    //Check for Windows1256
                    else if (IsWindows1256Character(Text[i]))
                    {
                        m_EciAssignmentNumber = 24;
                        break;
                    }
                }
            }
            #endregion

            #region Initialize Version and Error correction level

#if !XAML && !GDI
            if (!m_IsUserMentionedVersion || m_version == QRCodeVersion.Auto)
#else
            if (!m_IsUserMentionedVersion || m_version == QRBarcodeVersion.Auto)
#endif
            {
                int[] dataCapacityOfVersions = null;
                if (m_IsUserMentionedErrorCorrectionLevel)
                {
                    switch (m_inputMode)
                    {
#if !XAML && !GDI
                        case InputMode.NumericMode:
#else
                        case QRInputMode.NumericMode:
#endif
                            {
                                switch ((int)m_errorCorrectionLevel)
                                {
                                    case 7:
                                        dataCapacityOfVersions = PdfQRBarcodeValues.NumericDataCapacityLow;
                                        break;
                                    case 15:
                                        dataCapacityOfVersions = PdfQRBarcodeValues.NumericDataCapacityMedium;
                                        break;
                                    case 25:
                                        dataCapacityOfVersions = PdfQRBarcodeValues.NumericDataCapacityQuartile;
                                        break;
                                    case 30:
                                        dataCapacityOfVersions = PdfQRBarcodeValues.NumericDataCapacityHigh;
                                        break;
                                }
                                break;
                            }
#if !XAML && !GDI
                        case InputMode.AlphaNumericMode:
#else
                        case QRInputMode.AlphaNumericMode:
#endif
                            {
                                switch ((int)m_errorCorrectionLevel)
                                {
                                    case 7:
                                        dataCapacityOfVersions = PdfQRBarcodeValues.AlphanumericDataCapacityLow;
                                        break;
                                    case 15:
                                        dataCapacityOfVersions = PdfQRBarcodeValues.AlphanumericDataCapacityMedium;
                                        break;
                                    case 25:
                                        dataCapacityOfVersions = PdfQRBarcodeValues.AlphanumericDataCapacityQuartile;
                                        break;
                                    case 30:
                                        dataCapacityOfVersions = PdfQRBarcodeValues.AlphanumericDataCapacityHigh;
                                        break;
                                }
                                break;
                            }
#if !XAML && !GDI
                        case InputMode.BinaryMode:
#else
                        case QRInputMode.BinaryMode:
#endif
                            {
                                switch ((int)m_errorCorrectionLevel)
                                {
                                    case 7:
                                        dataCapacityOfVersions = PdfQRBarcodeValues.BinaryDataCapacityLow;
                                        break;
                                    case 15:
                                        dataCapacityOfVersions = PdfQRBarcodeValues.BinaryDataCapacityMedium;
                                        break;
                                    case 25:
                                        dataCapacityOfVersions = PdfQRBarcodeValues.BinaryDataCapacityQuartile;
                                        break;
                                    case 30:
                                        dataCapacityOfVersions = PdfQRBarcodeValues.BinaryDataCapacityHigh;
                                        break;
                                }
                                break;
                            }
                    }

                }
                else
                {
#if !XAML && !GDI
                    m_errorCorrectionLevel = PdfErrorCorrectionLevel.Low;
#else
                    m_errorCorrectionLevel = ErrorCorrectionLevel.Low;
#endif

                    switch (m_inputMode)
                    {
#if !XAML && !GDI
                        case InputMode.NumericMode:
#else
                        case QRInputMode.NumericMode:
#endif
                            {
                                dataCapacityOfVersions = PdfQRBarcodeValues.NumericDataCapacityLow;
                                break;
                            }
#if !XAML && !GDI
                        case InputMode.AlphaNumericMode:
#else
                        case QRInputMode.AlphaNumericMode:
#endif
                            {
                                dataCapacityOfVersions = PdfQRBarcodeValues.AlphanumericDataCapacityLow;
                                break;
                            }
#if !XAML && !GDI
                        case InputMode.BinaryMode:
#else
                        case QRInputMode.BinaryMode:
#endif
                            {
                                dataCapacityOfVersions = PdfQRBarcodeValues.BinaryDataCapacityLow;
                                break;
                            }
                    }
                }
                int i;
                for (i = 0; i < dataCapacityOfVersions.Length; i++)
                {
                    if (dataCapacityOfVersions[i] > Text.Length)
                        break;
                }
#if !XAML && !GDI
                Version = (QRCodeVersion)i + 1;
#else
                QRVersion = (QRBarcodeVersion)i + 1;
#endif
            }
            else if (m_IsUserMentionedVersion)
            {
                if (m_IsUserMentionedErrorCorrectionLevel)
                {
                    int capacity = 0;
#if !XAML && !GDI
                    if (m_inputMode == InputMode.AlphaNumericMode)
#else
                    if (m_inputMode == QRInputMode.AlphaNumericMode)
#endif
                        capacity = PdfQRBarcodeValues.GetAlphanumericDataCapacity(m_version, m_errorCorrectionLevel);
#if !XAML && !GDI
                    else if (m_inputMode == InputMode.NumericMode)
#else
                    else if (m_inputMode == QRInputMode.NumericMode)

#endif
                        capacity = PdfQRBarcodeValues.GetNumericDataCapacity(m_version, m_errorCorrectionLevel);
#if !XAML && !GDI
                    if (m_inputMode == InputMode.BinaryMode)
#else
                    if (m_inputMode == QRInputMode.BinaryMode)
#endif
                        capacity = PdfQRBarcodeValues.GetBinaryDataCapacity(m_version, m_errorCorrectionLevel);

                    if (capacity < Text.Length)
                    {
#if !XAML && !NETFX_CORE && !GDI && !WP
                        throw new PdfBarcodeException("Text length is greater than the version capacity");
#else
                        throw new BarcodeException("Text length is greater than the version capacity");
#endif
                    }
                }
                else
                {
                    int capacityLow = 0, capacityMedium = 0, capacityQuartile = 0, capacityHigh = 0;

#if !XAML && !GDI
                    if (m_inputMode == InputMode.AlphaNumericMode)
#else
                    if (m_inputMode == QRInputMode.AlphaNumericMode)
#endif
                    {
#if !XAML && !GDI
                        capacityLow = PdfQRBarcodeValues.GetAlphanumericDataCapacity(m_version, PdfErrorCorrectionLevel.Low);
                        capacityMedium = PdfQRBarcodeValues.GetAlphanumericDataCapacity(m_version, PdfErrorCorrectionLevel.Medium);
                        capacityQuartile = PdfQRBarcodeValues.GetAlphanumericDataCapacity(m_version, PdfErrorCorrectionLevel.Quartile);
                        capacityHigh = PdfQRBarcodeValues.GetAlphanumericDataCapacity(m_version, PdfErrorCorrectionLevel.High);
#else
                        capacityLow = PdfQRBarcodeValues.GetAlphanumericDataCapacity(m_version, ErrorCorrectionLevel.Low);
                        capacityMedium = PdfQRBarcodeValues.GetAlphanumericDataCapacity(m_version, ErrorCorrectionLevel.Medium);
                        capacityQuartile = PdfQRBarcodeValues.GetAlphanumericDataCapacity(m_version, ErrorCorrectionLevel.Quartile);
                        capacityHigh = PdfQRBarcodeValues.GetAlphanumericDataCapacity(m_version, ErrorCorrectionLevel.High);

#endif
                    }
#if !XAML && !GDI
                    else if (m_inputMode == InputMode.NumericMode)
#else
                    else if (m_inputMode == QRInputMode.NumericMode)
#endif
                    {
#if !XAML && !GDI
                        capacityLow = PdfQRBarcodeValues.GetNumericDataCapacity(m_version, PdfErrorCorrectionLevel.Low);
                        capacityMedium = PdfQRBarcodeValues.GetNumericDataCapacity(m_version, PdfErrorCorrectionLevel.Medium);
                        capacityQuartile = PdfQRBarcodeValues.GetNumericDataCapacity(m_version, PdfErrorCorrectionLevel.Quartile);
                        capacityHigh = PdfQRBarcodeValues.GetNumericDataCapacity(m_version, PdfErrorCorrectionLevel.High);
#else
                        capacityLow = PdfQRBarcodeValues.GetNumericDataCapacity(m_version, ErrorCorrectionLevel.Low);
                        capacityMedium = PdfQRBarcodeValues.GetNumericDataCapacity(m_version, ErrorCorrectionLevel.Medium);
                        capacityQuartile = PdfQRBarcodeValues.GetNumericDataCapacity(m_version, ErrorCorrectionLevel.Quartile);
                        capacityHigh = PdfQRBarcodeValues.GetNumericDataCapacity(m_version, ErrorCorrectionLevel.High);

#endif
                    }
#if !XAML && !GDI
                    else if (m_inputMode == InputMode.BinaryMode)
#else
                    else if (m_inputMode == QRInputMode.BinaryMode)
#endif
                    {
#if !XAML && !GDI
                        capacityLow = PdfQRBarcodeValues.GetBinaryDataCapacity(m_version, PdfErrorCorrectionLevel.Low);
                        capacityMedium = PdfQRBarcodeValues.GetBinaryDataCapacity(m_version, PdfErrorCorrectionLevel.Medium);
                        capacityQuartile = PdfQRBarcodeValues.GetBinaryDataCapacity(m_version, PdfErrorCorrectionLevel.Quartile);
                        capacityHigh = PdfQRBarcodeValues.GetBinaryDataCapacity(m_version, PdfErrorCorrectionLevel.High);
#else
                        capacityLow = PdfQRBarcodeValues.GetBinaryDataCapacity(m_version, ErrorCorrectionLevel.Low);
                        capacityMedium = PdfQRBarcodeValues.GetBinaryDataCapacity(m_version, ErrorCorrectionLevel.Medium);
                        capacityQuartile = PdfQRBarcodeValues.GetBinaryDataCapacity(m_version, ErrorCorrectionLevel.Quartile);
                        capacityHigh = PdfQRBarcodeValues.GetBinaryDataCapacity(m_version, ErrorCorrectionLevel.High);

#endif
                    }

                    if (capacityHigh > Text.Length)
#if !XAML && !GDI
                        m_errorCorrectionLevel = PdfErrorCorrectionLevel.High;
#else
                        m_errorCorrectionLevel = ErrorCorrectionLevel.High;
#endif

                    else if (capacityQuartile > Text.Length)
#if !XAML && !GDI
                        m_errorCorrectionLevel = PdfErrorCorrectionLevel.Quartile;
#else
                        m_errorCorrectionLevel = ErrorCorrectionLevel.Quartile;
#endif

                    else if (capacityMedium > Text.Length)
#if !XAML && !GDI
                        m_errorCorrectionLevel = PdfErrorCorrectionLevel.Medium;
#else
                        m_errorCorrectionLevel = ErrorCorrectionLevel.Medium;
#endif

                    else if (capacityLow > Text.Length)
#if !XAML && !GDI
                        m_errorCorrectionLevel = PdfErrorCorrectionLevel.Low;
#else
                        m_errorCorrectionLevel = ErrorCorrectionLevel.Low;
#endif

                    else
#if !XAML && !NETFX_CORE && !GDI && !WP
                        throw new PdfBarcodeException("Text length is greater than the version capacity");
#else
                        throw new BarcodeException("Text length is greater than the version capacity");
#endif
                }
            }
            #endregion
        }

        /// <summary>
        /// Splits the Code words
        /// </summary>
        /// <param name="ds">The Encoded value Blocks.</param>
        /// <param name="noOfBlocks">Index of Block Number.</param>
        /// <param name="count">Length of the Block.</param>
        private string[] SplitCodeWord(string[,] ds, int blk, int count)
        {
            string[] ds1 = new string[count];
            for (int i = 0; i < count; i++)
            {
                ds1[i] = ds[blk, i];
            }
            return ds1;
        }

        /// <summary>
        /// Creates the Blocks
        /// </summary>
        /// <param name="encodeData">The Encoded value.</param>
        /// <param name="noOfBlocks">Number of Blocks.</param>
        private string[,] CreateBlocks(List<bool> encodeData, int noOfBlocks)
        {
            string[,] ret = new string[noOfBlocks, encodeData.Count / 8 / noOfBlocks];
            string stringValue = null;
            int j = 0, i = 0, blockNumber = 0;

            for (; j < encodeData.Count; j++)
            {


                if (j % 8 == 0 && j != 0)
                {


                    ret[blockNumber, i] = stringValue;
                    stringValue = null;
                    i++;

                    if (i == (encodeData.Count / noOfBlocks / 8))
                    {
                        blockNumber++;
                        i = 0;
                    }
                }

                stringValue += encodeData[j] ? 1 : 0;
            }
            ret[blockNumber, i] = stringValue;
            return ret;
        }

        /// <summary>
        /// Converts Integer value to Boolean
        /// </summary>
        /// <param name="number">The Integer value.</param>
        /// <param name="noOfBits">Number of Bits.</param>
        private bool[] IntToBoolArray(int number, int noOfBits)
        {
            bool[] numberInBool = new bool[noOfBits];
            for (int i = 0; i < noOfBits; i++)
            {
                numberInBool[noOfBits - i - 1] = ((number >> i) & 1) == 1;
            }
            return numberInBool;
        }

        /// <summary>
        /// Converts string value to Boolean
        /// </summary>
        /// <param name="numberInString">The String value.</param>
        /// <param name="noOfBits">Number of Bits.</param>
        private bool[] StringToBoolArray(string numberInString, int noOfBits)
        {
            bool[] numberInBool = new bool[noOfBits];
            char[] dataStringArray = numberInString.ToCharArray();
            int number = 0;
            for (int i = 0; i < dataStringArray.Length; i++)
            {
                number = number * 10 + (int)dataStringArray[i] - 48;
            }

            for (int i = 0; i < noOfBits; i++)
            {
                numberInBool[noOfBits - i - 1] = ((number >> i) & 1) == 1;
            }
            return numberInBool;
        }

        /// <summary>
        /// Gets the Allignment pattern coordinates of the current version.
        /// </summary>
        private int[] GetAlignmentPatternCoOrdinates()
        {
            int[] allign = null;
            switch ((int)m_version)
            {
                case 02:
                    allign = new int[] { 6, 18 };
                    break;
                case 03:
                    allign = new int[] { 6, 22 };
                    break;
                case 04:
                    allign = new int[] { 6, 26 };
                    break;
                case 05:
                    allign = new int[] { 6, 30 };
                    break;
                case 06:
                    allign = new int[] { 6, 34 };
                    break;
                case 07:
                    allign = new int[] { 6, 22, 38 };
                    break;
                case 08:
                    allign = new int[] { 6, 24, 42 };
                    break;
                case 09:
                    allign = new int[] { 6, 26, 46 };
                    break;
                case 10:
                    allign = new int[] { 6, 28, 50 };
                    break;
                case 11:
                    allign = new int[] { 6, 30, 54 };
                    break;
                case 12:
                    allign = new int[] { 6, 32, 58 };
                    break;
                case 13:
                    allign = new int[] { 6, 34, 62 };
                    break;
                case 14:
                    allign = new int[] { 6, 26, 46, 66 };
                    break;
                case 15:
                    allign = new int[] { 6, 26, 48, 70 };
                    break;
                case 16:
                    allign = new int[] { 6, 26, 50, 74 };
                    break;
                case 17:
                    allign = new int[] { 6, 30, 54, 78 };
                    break;
                case 18:
                    allign = new int[] { 6, 30, 56, 82 };
                    break;
                case 19:
                    allign = new int[] { 6, 30, 58, 86 };
                    break;
                case 20:
                    allign = new int[] { 6, 34, 62, 90 };
                    break;
                case 21:
                    allign = new int[] { 6, 28, 50, 72, 94 };
                    break;
                case 22:
                    allign = new int[] { 6, 26, 50, 74, 98 };
                    break;
                case 23:
                    allign = new int[] { 6, 30, 54, 78, 102 };
                    break;
                case 24:
                    allign = new int[] { 6, 28, 54, 80, 106 };
                    break;
                case 25:
                    allign = new int[] { 6, 32, 58, 84, 110 };
                    break;
                case 26:
                    allign = new int[] { 6, 30, 58, 86, 114 };
                    break;
                case 27:
                    allign = new int[] { 6, 34, 62, 90, 118 };
                    break;
                case 28:
                    allign = new int[] { 6, 26, 50, 74, 98, 122 };
                    break;
                case 29:
                    allign = new int[] { 6, 30, 54, 78, 102, 126 };
                    break;
                case 30:
                    allign = new int[] { 6, 26, 52, 78, 104, 130 };
                    break;
                case 31:
                    allign = new int[] { 6, 30, 56, 82, 108, 134 };
                    break;
                case 32:
                    allign = new int[] { 6, 34, 60, 86, 112, 138 };
                    break;
                case 33:
                    allign = new int[] { 6, 30, 58, 86, 114, 142 };
                    break;
                case 34:
                    allign = new int[] { 6, 34, 62, 90, 118, 146 };
                    break;
                case 35:
                    allign = new int[] { 6, 30, 54, 78, 102, 126, 150 };
                    break;
                case 36:
                    allign = new int[] { 6, 24, 50, 76, 102, 128, 154 };
                    break;
                case 37:
                    allign = new int[] { 6, 28, 54, 80, 106, 132, 158 };
                    break;
                case 38:
                    allign = new int[] { 6, 32, 58, 84, 110, 136, 162 };
                    break;
                case 39:
                    allign = new int[] { 6, 26, 54, 82, 110, 138, 166 };
                    break;
                case 40:
                    allign = new int[] { 6, 30, 58, 86, 114, 142, 170 };
                    break;

            }
            return allign;
        }

        /// <summary>
        /// Allocates Format and Version Information
        /// </summary>
        private void AllocateFormatAndVersionInformation()
        {
            for (int i = 0; i < 9; i++)
            {
                m_moduleValue[8, i].IsFilled = true;
                m_moduleValue[i, 8].IsFilled = true;
            }
            for (int i = m_noOfModules - 8; i < m_noOfModules; i++)
            {
                m_moduleValue[8, i].IsFilled = true;
                m_moduleValue[i, 8].IsFilled = true;
            }
            if ((int)m_version > 6)
            {
                byte[] versionInformation = m_qrBarcodeValues.VersionInformation;
                int count = 0;
                for (int i = 0; i < 6; i++)
                    for (int j = 2; j >= 0; j--)
                    {
                        m_moduleValue[i, m_noOfModules - 9 - j].IsBlack = versionInformation[count] == 1 ? true : false;
                        m_moduleValue[i, m_noOfModules - 9 - j].IsFilled = true;

                        m_moduleValue[m_noOfModules - 9 - j, i].IsBlack = versionInformation[count++] == 1 ? true : false;
                        m_moduleValue[m_noOfModules - 9 - j, i].IsFilled = true;
                    }


            }
        }

        #region Detect Language
        private bool IsCP437Character(char inputChar)
        {
            string inputCharInHex = ((int)inputChar).ToString("X");

            //Hexadecimal values of CP437 characters
            string[] CP437CharSet = { "2591", "2592", "2593", "2502", "2524", "2561", "2562", "2556", "2555", "2563", "2551", "2557", "255D", "255C", "255B", "2510", "2514", "2534", "252C", "251C", "2500", "253C", "255E", "255F", "255A", "2554", "2569", "2566", "2560", "2550", "256C", "2567", "2568", "2564", "2565", "2559", "2558", "2552", "2553", "256B", "256A", "2518", "250C", "2588", "2584", "258C", "2590", "2580", "25A0" };
            if (Array.IndexOf(CP437CharSet, inputCharInHex) > -1)
                return true;
            return false;
        }

        private bool IsISO8859_2Character(char inputChar)
        {
            string inputCharInHex = ((int)inputChar).ToString("X");

            //Hexadecimal values of Latin2 characters
            string[] latin2CharSet = { "104", "2D8", "141", "13D", "15A", "160", "15E", "164", "179", "17D", "17B", "105", "2DB", "142", "13E", "15B", "2C7", "161", "15F", "165", "17A", "2DD", "17E", "17C", "154", "102", "139", "106", "10C", "118", "11A", "10E", "110", "143", "147", "150", "158", "16E", "170", "162", "155", "103", "13A", "107", "10D", "119", "11B", "10F", "111", "144", "148", "151", "159", "16F", "171", "163", "2D9" };
            if (Array.IndexOf(latin2CharSet, inputCharInHex) > -1)
                return true;
            return false;
        }

        private bool IsISO8859_3Character(char inputChar)
        {
            string inputCharInHex = ((int)inputChar).ToString("X");

            //Hexadecimal values of Latin3 characters
            string[] latin3CharSet = { "126", "124", "130", "15E", "11E", "134", "17B", "127", "125", "131", "15F", "11F", "135", "17C", "10A", "108", "120", "11C", "16C", "15C", "10B", "109", "121", "11D", "16D", "15D" };
            if (Array.IndexOf(latin3CharSet, inputCharInHex) > -1)
                return true;
            return false;
        }

        private bool IsISO8859_4Character(char inputChar)
        {
            string inputCharInHex = ((int)inputChar).ToString("X");

            //Hexadecimal values of Latin4 characters
            string[] latin4CharSet = { "104", "138", "156", "128", "13B", "160", "112", "122", "166", "17D", "105", "2DB", "157", "129", "13C", "2C7", "161", "113", "123", "167", "14A", "17E", "14B", "100", "12E", "10C", "118", "116", "12A", "110", "145", "14C", "136", "172", "168", "16A", "101", "12F", "10D", "119", "117", "12B", "111", "146", "14D", "137", "173", "169", "16B" };
            if (Array.IndexOf(latin4CharSet, inputCharInHex) > -1)
                return true;
            return false;
        }

        private bool IsISO8859_5Character(char inputChar)
        {
            if ((int)inputChar >= 1025 && (int)inputChar <= 1119 &&
                (int)inputChar != 1037 && (int)inputChar != 1104 && (int)inputChar != 1117)
                return true;

            return false;
        }

        private bool IsISO8859_6Character(char inputChar)
        {
            if (((int)inputChar >= 1569 && (int)inputChar <= 1594) ||
                ((int)inputChar >= 1600 && (int)inputChar <= 1618) ||
                  (int)inputChar == 1567 || (int)inputChar == 1563 || (int)inputChar == 1548)
                return true;

            return false;
        }

        private bool IsISO8859_7Character(char inputChar)
        {
            if (((int)inputChar >= 900 && (int)inputChar <= 974) || (int)inputChar == 890)
                return true;

            return false;
        }

        private bool IsISO8859_8Character(char inputChar)
        {
            if (((int)inputChar >= 1488 && (int)inputChar <= 1514))
                return true;

            return false;
        }

        private bool IsISO8859_11Character(char inputChar)
        {
            if (((int)inputChar >= 3585 && (int)inputChar <= 3675))
                return true;

            return false;
        }

        private bool IsWindows1250Character(char inputChar)
        {
            string inputCharInHex = ((int)inputChar).ToString("X");

            //Hexadecimal values of Windows1250 characters
            string[] windows1250CharSet = { "141", "104", "15E", "17B", "142", "105", "15F", "13D", "13E", "17C" };

            if (Array.IndexOf(windows1250CharSet, inputCharInHex) > -1)
                return true;
            if ((int)inputChar >= 1569 && (int)inputChar <= 1610)
                return true;
            return false;
        }

        private bool IsWindows1251Character(char inputChar)
        {
            string inputCharInHex = ((int)inputChar).ToString("X");

            //Hexadecimal values of Windows1251 characters
            string[] windows1251CharSet = { "402", "403", "453", "409", "40A", "40C", "40B", "40F", "452", "459", "45A", "45C", "45B", "45F", "40E", "45E", "408", "490", "401", "404", "407", "406", "456", "491", "451", "454", "458", "405", "455", "457" };

            if (Array.IndexOf(windows1251CharSet, inputCharInHex) > -1)
                return true;
            if ((int)inputChar >= 1040 && (int)inputChar <= 1103)
                return true;
            return false;
        }

        private bool IsWindows1252Character(char inputChar)
        {
            string inputCharInHex = ((int)inputChar).ToString("X");

            //Hexadecimal values of Windows1252 characters
            string[] windows1252CharSet = { "20AC", "201A", "192", "201E", "2026", "2020", "2021", "2C6", "2030", "160", "2039", "152", "17D", "2018", "2019", "201C", "201D", "2022", "2013", "2014", "2DC", "2122", "161", "203A", "153", "17E", "178" };

            if (Array.IndexOf(windows1252CharSet, inputCharInHex) > -1)
                return true;
            return false;
        }

        private bool IsWindows1256Character(char inputChar)
        {
            string inputCharInHex = ((int)inputChar).ToString("X");

            //Hexadecimal values of Windows1256 characters
            string[] windows1256CharSet = { "67E", "679", "152", "686", "698", "688", "6AF", "6A9", "691", "153", "6BA", "6BE", "6C1" };

            if (Array.IndexOf(windows1256CharSet, inputCharInHex) > -1)
                return true;
            if ((int)inputChar >= 1569 && (int)inputChar <= 1610)
                return true;
            return false;
        }

        #endregion
        #endregion
    }
}
#endif