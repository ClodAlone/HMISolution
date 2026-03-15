#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using Syncfusion.XlsIO.Implementation;
namespace Syncfusion.ExcelToPdfConverter
{
    internal class ExcelToPdfLayoutSetting
    {
        #region Constants
        private ExcelToPdfConverter m_excelToPdfConverter;
        private float m_headerMargin;
        private float m_footerMargin;
        private float m_topMargin;
        private float m_bottomMargin;
        private float m_leftMargin;
        private float m_rightMargin;
        private float m_sheetWidth;
        #endregion
        
        #region constructor
        /// <summary>
        /// To assign the Excel to pdf converter
        /// </summary>
        /// <param name="excelToPdfConverter"></param>
        internal ExcelToPdfLayoutSetting(ExcelToPdfConverter excelToPdfConverter)
        {
            m_excelToPdfConverter = excelToPdfConverter;
            m_bottomMargin = excelToPdfConverter.BottomMargin;
            m_footerMargin = excelToPdfConverter.FooterMargin;
            m_topMargin = excelToPdfConverter.TopMargin;
            m_headerMargin = excelToPdfConverter.HeaderMargin;
            m_leftMargin = excelToPdfConverter.LeftMargin;
            m_rightMargin = excelToPdfConverter.RightMargin;
            m_sheetWidth = excelToPdfConverter.sheetWidth;
        }
        #endregion

        #region Properties
        internal ExcelToPdfConverter ExcelToPdf
        {
            get
            {
                return m_excelToPdfConverter;
            }
        }
        #endregion

        #region Methods
        internal void FitSheetOnPage(PdfSection pdfSection, IPageSetup SheetPageSetup, float usedRangeWidth, float usedRangeHeight)
        {
            float sheetHeight;
            float sheetWidth;
            PdfPageSettings pageSettings = pdfSection.PageSettings;
            pageSettings.Orientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), SheetPageSetup.Orientation.ToString(), true);
            
            pageSettings.Size = ExcelToPdfConverterSettings.GetExcelSheetSize(SheetPageSetup.PaperSize);
            
            PdfTemplate pdfPageTemplate = new PdfTemplate(usedRangeWidth +
                                                   (pageSettings.Margins.Left + pageSettings.Margins.Right),
                                                   usedRangeHeight + (m_topMargin + m_footerMargin + m_bottomMargin));
            sheetHeight = usedRangeHeight +
                          (m_topMargin + m_bottomMargin);
            sheetWidth = pdfPageTemplate.Width;
            if (SheetPageSetup.Orientation == ExcelPageOrientation.Landscape)
            {
                sheetHeight = sheetHeight + m_footerMargin + m_headerMargin;
                sheetWidth = sheetWidth + m_leftMargin + m_rightMargin;
            }            
            ExcelToPdf.SheetHeight = sheetHeight;
            ExcelToPdf.sheetWidth = sheetWidth;            
        }
        /// <summary>
        /// It reture the pdftemplate for FitAllcolumnOnOnePage.
        /// </summary>
        /// <param name="pdfSection"></param>
        /// <param name="SheetPageSetup"></param>
        /// <param name="usedRangeWidth"></param>
        /// <param name="usedRangeHeight"></param>
        /// <returns></returns>
        internal void FitAllColumnOnOnePage(PdfSection pdfSection, IPageSetup SheetPageSetup, float usedRangeWidth, float usedRangeHeight)
        {
            float sheetHeight;
            float sheetWidth;
            PdfPageSettings pageSettings = pdfSection.PageSettings;
            pageSettings.Orientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), SheetPageSetup.Orientation.ToString(), true);

            pageSettings.Size = ExcelToPdfConverterSettings.GetExcelSheetSize(SheetPageSetup.PaperSize);


            sheetWidth = (float)Math.Ceiling(usedRangeWidth + (pageSettings.Margins.Left + pageSettings.Margins.Right));
            if (sheetWidth < pageSettings.Size.Width)
            {
                sheetWidth = pageSettings.Size.Width;
            }

            sheetHeight = ExcelToPdf.RequiredHeight(pageSettings.Size.Width, sheetWidth, pageSettings.Size.Height);
            if (sheetWidth > sheetHeight && !(ExcelToPdf.ExcelToPdfPagesetup.HasPrintTitleColumns || ExcelToPdf.ExcelToPdfPagesetup.HasPrintTitleRows))
            {
                sheetWidth += (m_leftMargin + m_rightMargin);
                sheetHeight -= (m_topMargin + m_bottomMargin);
            }
            else if (sheetHeight < usedRangeHeight)
            {
                sheetHeight -= (m_topMargin + m_bottomMargin);
            }            
            ExcelToPdf.SheetHeight = sheetHeight;
            ExcelToPdf.SheetWidth = sheetWidth;            
        }
        /// <summary>
        /// It's Return the Pdf Template for FitAllrowsOnOnePage.
        /// </summary>
        /// <param name="pdfSection"></param>
        /// <param name="SheetPageSetup"></param>
        /// <param name="usedRangeWidth"></param>
        /// <param name="usedRangeHeight"></param>
        /// <returns></returns>
        internal void FitAllRowsOnOnePage(PdfSection pdfSection, IPageSetup SheetPageSetup, float usedRangeWidth, float usedRangeHeight)
        {
            float sheetHeight;
            float sheetWidth;
            PdfPageSettings pageSettings = pdfSection.PageSettings;
            pageSettings.Orientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), SheetPageSetup.Orientation.ToString(), true);

            pageSettings.Size = ExcelToPdfConverterSettings.GetExcelSheetSize(SheetPageSetup.PaperSize);

            sheetHeight = usedRangeHeight + m_topMargin + m_bottomMargin;
            sheetWidth = pageSettings.Size.Width + pageSettings.Margins.Left + pageSettings.Margins.Right;
            if (sheetWidth < pageSettings.Size.Width)
            {
                sheetWidth = pageSettings.Size.Width - (pageSettings.Margins.Left + pageSettings.Margins.Right);
            }            
            ExcelToPdf.SheetHeight = sheetHeight;
            ExcelToPdf.sheetWidth = sheetWidth;            
        }
        /// <summary>
        /// NoScaling Layout option
        /// </summary>
        /// <param name="pdfSection"></param>
        /// <param name="SheetPageSetup"></param>
        /// <param name="usedRangeWidth"></param>
        /// <param name="usedRangeHeight"></param>
        internal void NoScaling(PdfSection pdfSection, IPageSetup SheetPageSetup, float usedRangeWidth, float usedRangeHeight)
        {
            PdfPageSettings pageSettings = pdfSection.PageSettings;
            pageSettings.Orientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), SheetPageSetup.Orientation.ToString(), true);
            pageSettings.Size = ExcelToPdfConverterSettings.GetExcelSheetSize(SheetPageSetup.PaperSize);

            ExcelToPdf.SheetHeight = pageSettings.Height - (m_topMargin + m_bottomMargin);

            ExcelToPdf.sheetWidth = pageSettings.Width - (m_leftMargin + m_rightMargin);   
        }
        internal void CustomScaling(PdfSection pdfSection, IPageSetup SheetPageSetup, float usedRangeWidth, float usedRangeHeight, IRange[] printAreas)
        {
            float sheetHeight;
            float sheetWidth;
            PdfPageSettings pageSettings = pdfSection.PageSettings;
            pageSettings.Orientation = (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation), SheetPageSetup.Orientation.ToString(), true);
            pageSettings.Size = ExcelToPdfConverterSettings.GetExcelSheetSize(SheetPageSetup.PaperSize);
            int zoomValue = SheetPageSetup.Zoom;

            PageSetupBaseImpl pageSetup = SheetPageSetup as PageSetupBaseImpl;
            SizeF sheetSize = new SizeF(pageSettings.Width, pageSettings.Height);

            if (pageSetup.IsFitToPage)
            {
                sheetWidth = pageSettings.Width;
                sheetHeight = pageSettings.Height;
                if (pageSetup.FitToPagesWide == 1 && sheetWidth < usedRangeWidth)
                {
                    sheetWidth = usedRangeWidth + (m_leftMargin + m_rightMargin);
                }
                else if (pageSetup.FitToPagesWide > 1)
                {
                    sheetWidth = sheetWidth * pageSetup.FitToPagesWide + (m_leftMargin + m_rightMargin);
                }
                if (pageSetup.FitToPagesTall == 1 && sheetHeight < usedRangeHeight)
                {
                    sheetHeight = usedRangeHeight + (m_topMargin + m_bottomMargin);
                }
                else if (pageSetup.FitToPagesTall > 1)
                {
                    sheetHeight = ExcelToPdf.RequiredHeight(pageSettings.Size.Width, sheetWidth, pageSettings.Size.Height);
                    sheetHeight -= (m_topMargin + m_bottomMargin);
                }

            }
            else
            {
                sheetHeight = pageSettings.Height - (m_topMargin + m_bottomMargin);
                sheetWidth = pageSettings.Width - (m_leftMargin + m_rightMargin);
            }            
            ExcelToPdf.SheetHeight = sheetHeight;
            ExcelToPdf.sheetWidth = sheetWidth;            
        }
        #endregion
    }
}
