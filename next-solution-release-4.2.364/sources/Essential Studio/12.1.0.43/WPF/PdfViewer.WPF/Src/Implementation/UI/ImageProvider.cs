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
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using Syncfusion.Windows.PdfViewer;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Graphics;
using System.Windows.Media;

namespace Syncfusion.Windows.PdfViewer
{
    internal class ImageProvider : ItemsProvider<IEnumerable<System.Windows.Controls.Image>>
    {
        # region Fields
        internal PdfLoadedDocument m_view;
        DocumentView m_docViewer;
        Dictionary<int, Stream> m_imgStream = new Dictionary<int, Stream>();
        private int m_currentPage = 0;
        #endregion

        # region Constructor
        public ImageProvider(PdfLoadedDocument viewer, DocumentView docView)
        {
            m_view = viewer;
            m_docViewer = docView;
        }
        # endregion

        # region Properties
        public int CurrentPage
        {
            get
            {
                return m_currentPage;
            }
            set
            {
                m_currentPage = value;
            }
        }
        # endregion

        # region Methods
        internal BitmapSource ExportAsImage(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= this.m_docViewer.Pages.Length)
                throw new IndexOutOfRangeException("Page index is not inside the range of the pages");

            DrawingVisual dv = this.m_docViewer.Pages[pageIndex].Graphics.Visual;

            int actualWidth = (int)m_docViewer.UnitConvertor.ConvertToPixels((float)(this.m_docViewer.Pages[pageIndex].ActualWidth), PdfGraphicsUnit.Point);
            int actualHeight = (int)m_docViewer.UnitConvertor.ConvertToPixels((float)(this.m_docViewer.Pages[pageIndex].ActualHeight), PdfGraphicsUnit.Point);

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(actualWidth, actualHeight, 96, 96, PixelFormats.Pbgra32);
            renderBitmap.Render(dv);

            return renderBitmap;
        }
        
        /// <summary>
        /// Exports the specified pages as Images
        /// </summary>
        /// <param name="startIndex">The starting page index</param>
        /// <param name="endIndex">The ending page index</param>
        /// <returns>Returns the specified pages as Images</returns>
        public BitmapSource[] ExportAsImage(int startIndex, int endIndex)
        {
            if (endIndex < startIndex)
                throw new ArgumentException("Invalid arguments-Start index cannot be greater than end index");

            if (startIndex < 0 && endIndex >= this.m_docViewer.Pages.Length)
                throw new IndexOutOfRangeException("The specified index is not inside the bounds of the page range");

            BitmapSource[] bitmapCollection = new BitmapSource[endIndex - startIndex + 1];

            for (int i = startIndex; i <= endIndex; i++)
            {
                DrawingVisual dv = this.m_docViewer.Pages[i].Graphics.Visual;

                PdfUnitConvertor m_unitConvertor = new PdfUnitConvertor();
                int actualWidth = (int)m_docViewer.UnitConvertor.ConvertToPixels((float)(this.m_docViewer.Pages[i].ActualWidth), PdfGraphicsUnit.Point);
                int actualHeight = (int)m_docViewer.UnitConvertor.ConvertToPixels((float)(this.m_docViewer.Pages[i].ActualHeight), PdfGraphicsUnit.Point);

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(actualWidth, actualHeight, 96, 96, PixelFormats.Pbgra32);
                renderBitmap.Render(dv);
                bitmapCollection[i] = renderBitmap;
            }

            return bitmapCollection;
        }

        public int Count()
        {
            return m_docViewer.PageCount;
        }

        public BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        public void Unload()
        {
            foreach (KeyValuePair<int, Stream> val in m_imgStream)
            {
                val.Value.Flush();
                val.Value.Dispose();
            }
            m_imgStream.Clear();
        }
        public IList<IEnumerable<System.Windows.Controls.Image>> GetBlankPage(int startIndex)
        {
            var list = new List<IEnumerable<System.Windows.Controls.Image>>();
            var rowList = new List<System.Windows.Controls.Image>(1);
            Bitmap bit = null;
            if (m_docViewer.BlankImageStream.Length == 0)
            {
                Page currentPage = m_docViewer.Pages[startIndex];
                bit = new Bitmap((int)currentPage.Width, (int)currentPage.Height);
                for (int x = 0; x < (int)currentPage.Width; ++x)
                {
                    for (int y = 0; y < (int)currentPage.Height; ++y)
                    {
                        bit.SetPixel(x, y, System.Drawing.Color.White);
                    }
                }
                bit.Save(m_docViewer.BlankImageStream, ImageFormat.Jpeg);
            }
            else
            {
                bit = new Bitmap(m_docViewer.BlankImageStream);
            }
            BitmapSource bms = bit.ToBitmapSource();
            bms.Freeze();
            bit.Dispose();
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Source = bms;
            rowList.Add(img);
            list.Add(rowList);
            return list;
        }
        public IList<IEnumerable<System.Windows.Controls.Image>> GetRange(int startIndex, int count)
        {
            var imagesPerRow = 1;
            var end = Math.Min(Count(), startIndex + count - 1);
            var list = new List<IEnumerable<System.Windows.Controls.Image>>();
            var rowList = new List<System.Windows.Controls.Image>(1);
            int i = startIndex;
            //for (int i = Math.Min(Count(), startIndex); i <= Math.Min(Count(), Math.Max(startIndex, end)); i++)
            {
                if (i >= 0)
                {
                    BitmapSource bms = m_docViewer.DisplaybyAddingControl(i);

                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                    img.Source = bms;
                    rowList.Add(img);

                    if (rowList.Count % imagesPerRow == 0 || i == end)
                    {
                        list.Add(rowList);

                        if (i == end && rowList.Count % imagesPerRow != 0)
                        {
                            var last = rowList.Last();
                            last.Margin = new Thickness(0);
                        }

                        if (i < end)
                            rowList = new List<System.Windows.Controls.Image>(imagesPerRow);
                    }
                }
            }
            return list;
        }
        # endregion
    }

    public enum ImageRotation
    {
        None,
        Rotate90,
        Rotate180,
        Rotate270
    }
    internal static class BitmapExtensionMethods
    {
        public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap bmp)
        {
            var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            int bufferSize = bmpData.Stride * bmp.Height;
            var bms = new System.Windows.Media.Imaging.WriteableBitmap(bmp.Width, bmp.Height, bmp.HorizontalResolution, bmp.VerticalResolution, PixelFormats.Bgr32, null);
            bms.WritePixels(new Int32Rect(0, 0, bmp.Width, bmp.Height), bmpData.Scan0, bufferSize, bmpData.Stride);
            bmp.UnlockBits(bmpData);

            return bms;
        }
    }
}
