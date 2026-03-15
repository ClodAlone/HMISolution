#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !(SILVERLIGHT || WP)
using System.Drawing;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.PivotTables
{
    /// <summary>
    /// Class used for rendering the Pivot Table style in the PDF page.
    /// </summary>
    public class PivotTableStyleRenderer
    {
        /// <summary>
        /// Represents the worksheet object
        /// </summary>
        private IWorksheet wkSheet;
       

        internal PivotTableStyleRenderer()
        {
        }

        internal PivotTableStyleRenderer(IWorksheet worksheet)
        {
            wkSheet = worksheet;

        }

        internal ExtendedFormatImpl ApplyStyles(PivotBuiltInStyles? BuildinStyle, PivotTableParts tableParts)
        {
            ExtendedFormatImpl EX = new ExtendedFormatImpl(wkSheet.Application, wkSheet.Workbook);
            WorkbookImpl m_book = wkSheet.Workbook as WorkbookImpl;
            FontImpl font = (FontImpl)m_book.CreateFont(null, false);

            font = (FontImpl)m_book.InnerFonts.Add(font);
            EX.FontIndex = font.Font.Index;
            switch (BuildinStyle)
            {
                case PivotBuiltInStyles.PivotStyleLight1:
                    #region PivtoStyleLight1
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(217, 217, 217);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Color = Color.FromArgb(255, 255, 255);
                        EX.PatternColor = Color.FromArgb(255, 255, 255);

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;

                case PivotBuiltInStyles.PivotStyleLight2:
                    #region PivtoStyleLight2
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(79, 129, 189);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(79, 129, 189);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(79, 129, 189);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(255, 255, 255);
                        EX.Color = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(79, 129, 189);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight3:
                    #region PivtoStyleLight3
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(242, 220, 219);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(192, 80, 77);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(192, 80, 77);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(192, 80, 77);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(255, 255, 255);
                        EX.Color = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(192, 80, 77);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight4:
                    #region PivtoStyleLight4
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(235, 241, 222);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(155, 187, 89);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(155, 187, 89);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(155, 187, 89);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(255, 255, 255);
                        EX.Color = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(155, 187, 89);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight5:
                    #region PivtoStyleLight5
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(228, 223, 236);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 100, 162);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 100, 162);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 100, 162);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(255, 255, 255);
                        EX.Color = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 100, 162);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight6:
                    #region PivtoStyleLight6
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 238, 243);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(75, 172, 198);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(75, 172, 198);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(75, 172, 198);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(255, 255, 255);
                        EX.Color = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(75, 172, 198);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight7:
                    #region PivtoStyleLight7
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(253, 233, 217);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(247, 150, 70);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(247, 150, 70);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(247, 150, 70);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(255, 255, 255);
                        EX.Color = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(247, 150, 70);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;

                case PivotBuiltInStyles.PivotStyleLight8:
                    #region PivtoStyleLight8
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(217, 217, 217);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);

                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(255, 255, 255);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight9:
                    #region PivtoStyleLight9
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(79, 129, 189);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(79, 129, 189);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(79, 129, 189);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(79, 129, 189);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.Color = Color.FromArgb(220, 230, 241);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(184, 204, 228);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(184, 204, 228);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(220, 230, 241);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.ColumnSubHeading3 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(220, 230, 241);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.Color = Color.FromArgb(220, 230, 241);

                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(79, 129, 189);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(79, 129, 189);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(79, 129, 189);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight10:
                    #region PivtoStyleLight10
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(242, 220, 219);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(192, 80, 77);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(192, 80, 77);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(192, 80, 77);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(192, 80, 77);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Color = Color.FromArgb(242, 220, 219);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading2 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.Font.FontName = "Calibri";
                        //EX.Font.Size = 11;
                        //EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(242, 220, 219);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.ColumnSubHeading3 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(242, 220, 219);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Color = Color.FromArgb(242, 220, 219);

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(192, 80, 77);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(192, 80,77);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(192, 80,77);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight11:
                    #region PivtoStyleLight11
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(235, 241, 222);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(155, 187, 89);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(155, 187, 89);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(155, 187, 89);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(155, 187, 89);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(216, 228, 188);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(216, 228, 188);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading2 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.Font.FontName = "Calibri";
                        //EX.Font.Size = 11;
                        //EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(235, 241, 222);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.ColumnSubHeading3 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(235, 241, 222);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(155, 187, 89);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(155, 187,89);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(155, 187,89);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight12:
                    #region PivtoStyleLight12
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(228, 223, 236);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 100, 162);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 100, 162);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 100, 162);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 100, 162);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Color = Color.FromArgb(228, 223, 236);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(204, 192, 218);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(204, 192, 218);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading2 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.Font.FontName = "Calibri";
                        //EX.Font.Size = 11;
                        //EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(228, 223, 236);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.ColumnSubHeading3 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(228, 223, 236);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 100, 162);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 100, 162);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 100, 162);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight13:
                    #region PivtoStyleLight13
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 238, 243);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(75, 172, 198);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(75, 172, 198);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(75, 172, 198);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(75, 172, 198);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Color = Color.FromArgb(218, 238, 243);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(183, 222, 232);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(183, 222, 232);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading2 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.Font.FontName = "Calibri";
                        //EX.Font.Size = 11;
                        //EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(218, 238, 243);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.ColumnSubHeading3 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(218, 238, 243);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Color = Color.FromArgb(218, 238, 243);

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(75, 172, 198);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(75, 172, 198);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(75, 172, 198);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight14:
                    #region PivtoStyleLight14
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(253, 233, 217);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(247, 150, 70);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(247, 150, 70);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(247, 150, 70);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(247, 150, 70);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Color = Color.FromArgb(253, 233, 217);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading2 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.Font.FontName = "Calibri";
                        //EX.Font.Size = 11;
                        //EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(253, 233, 217);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.ColumnSubHeading3 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(253, 233, 217);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Color = Color.FromArgb(253, 233, 217);

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(247, 150, 70);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(75, 172, 198);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(75, 172, 198);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight15:
                    #region PivtoStyleLight15
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);

                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(166, 166, 166);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight16:
                    #region PivtoStyleLight16
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(220, 230, 241); 
                        EX.Color = Color.FromArgb(220, 230, 241); 
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(149, 179, 215);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(217, 217, 217);                      
                        EX.Color = Color.FromArgb(217, 217, 217);   
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(79, 129, 189);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(149, 179, 215);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Color = Color.FromArgb(220, 230, 241);
                        EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.FillPattern = ExcelPattern.Solid;

                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(149, 179, 215);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight17:
                    #region PivtoStyleLight17
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Color = Color.FromArgb(242, 220, 219);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 150, 148);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(192, 80, 77);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 150, 148);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Color = Color.FromArgb(242, 220, 219);

                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(218, 150, 148);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight18:
                    #region PivtoStyleLight18
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(155, 187, 89);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);

                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(196, 215, 155);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight19:
                    #region PivtoStyleLight19
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Color = Color.FromArgb(228, 223, 236);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(177, 160, 199);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 100, 162);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(177, 160, 199);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Color = Color.FromArgb(228, 223, 236);

                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(177, 160, 199);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight20:
                    #region PivtoStyleLight20
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Color = Color.FromArgb(218, 238, 243);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(146, 205, 220);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(75, 172, 198);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(146, 205, 220);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Color = Color.FromArgb(218, 238, 243);

                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(146, 205, 220);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight21:
                    #region PivtoStyleLight21
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Color = Color.FromArgb(253, 233, 217);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(250, 191, 143);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(247, 150, 70);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(250, 191, 143);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Color = Color.FromArgb(253, 233, 217);

                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(250, 191, 143);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles .PivotStyleLight22:
                    #region PivtoStyleLight22
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(166, 166, 166);
                        EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(166, 166, 166);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight23:
                    #region PivtoStyleLight23
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;                        
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(79, 129, 189);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;                  
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;    
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;     
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;                
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;                    
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;                     
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(79, 129, 189);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight24:
                    #region PivtoStyleLight24
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;                        
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(192, 80, 77);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;                  
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;    
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;     
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;                
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;                    
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(192, 80, 77);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight25:
                    #region PivtoStyleLight25
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(155, 187, 89);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(155, 187, 89);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight26:
                    #region PivtoStyleLight26
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 100, 162);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 100, 162);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight27:
                    #region PivtoStyleLight27
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(75, 172, 198);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(75, 172, 198);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleLight28:
                    #region PivtoStyleLight28
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(247, 150, 70);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }

                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(247, 150, 70);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium1:
                    #region PivotStyleMedium1
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(217, 217, 217);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(128, 128, 128);                        
                        EX.Color = Color.FromArgb(128, 128, 128);
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(166, 166, 166);
                        EX.Color = Color.FromArgb(166, 166, 166);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        // EX.Font.RGBColor =  Color.FromArgb(0, 0, 0);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(217, 217, 217);
                        ////EX.PatternColor = Color.FromArgb(54, 96, 145);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(166, 166, 166);
                        EX.Color = Color.FromArgb(166, 166, 166);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(217, 217, 217);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(217, 217, 217);
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Hair;
                    }

                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0); 
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium2:
                    #region PivotStyleMedium2
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(54, 96, 146);
                        EX.Color = Color.FromArgb(54, 96, 146);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(54, 96, 146);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.PatternColor = Color.FromArgb(149, 179, 215);
                        EX.Color = Color.FromArgb(149, 179, 215);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        // EX.Font.RGBColor =  Color.FromArgb(0, 0, 0);
                        EX.PatternColor = Color.FromArgb(54, 96, 145);
                        EX.Color = Color.FromArgb(54, 96, 145);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(149, 179, 215);
                        EX.Color = Color.FromArgb(149, 179, 215);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                        EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.Color = Color.FromArgb(220, 230, 241);                        
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Hair;
                    }

                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(54, 96, 146);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium3:
                    #region PivotStyleMedium3
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(242, 220, 219);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(150, 54, 52);
                        EX.Color = Color.FromArgb(150, 54, 52);                      
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(218, 150, 148);
                        EX.Color = Color.FromArgb(218, 150, 148);                        
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);                       
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        // EX.Font.RGBColor =  Color.FromArgb(0, 0, 0);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(230, 184, 183);
                        // EX.PatternColor = Color.FromArgb(54, 96, 145);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(218, 150, 148);
                        EX.Color = Color.FromArgb(218, 150, 148);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(242, 220, 219);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                        EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Color = Color.FromArgb(242, 220, 219);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Hair;
                    }

                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(150, 54, 52);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium4:
                    #region PivotStyleMedium4
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(235, 241, 222);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(118, 147, 60);
                        EX.Color = Color.FromArgb(118, 147, 60);
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(196, 215, 155);
                        EX.Color = Color.FromArgb(196, 215, 155);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(216, 228, 188);
                        //EX.PatternColor = Color.FromArgb(54, 96, 145);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Color = Color.FromArgb(196, 215, 155);
                        EX.PatternColor = Color.FromArgb(196, 215, 155);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(235, 241, 222);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Hair;
                    }

                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(118, 147, 60);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium5:
                    #region PivotStyleMedium5
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(228, 223, 236);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(96, 72, 122);
                        EX.Color = Color.FromArgb(96, 72, 122);
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(177, 160, 199);
                        EX.Color = Color.FromArgb(177, 160, 199);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(204, 192, 218);
                        //EX.PatternColor = Color.FromArgb(54, 96, 145);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(177, 160, 199);
                        EX.Color = Color.FromArgb(177, 160, 199);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(228, 223, 236);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                        EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Color = Color.FromArgb(228, 223, 236);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Hair;
                    }

                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(96, 73, 122);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium6:
                    #region PivotStyleMedium6
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 238, 243);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(49, 134, 155);
                        EX.Color = Color.FromArgb(49, 134, 155);
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(146, 205, 220);
                        EX.Color = Color.FromArgb(146, 205, 220);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(183, 222, 232);
                        //EX.PatternColor = Color.FromArgb(54, 96, 145);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(146, 205, 220);
                        EX.Color = Color.FromArgb(146, 205, 220);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 238, 243);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                        EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Color = Color.FromArgb(218, 238, 243);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Hair;
                    }

                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(49, 134, 155);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium7:
                    #region PivotStyleMedium7
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(253, 233, 217);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(226, 107, 10);
                        EX.Color = Color.FromArgb(226, 107, 10);
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(250, 191, 143);
                        EX.Color = Color.FromArgb(250, 191, 143);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(252, 213, 180);
                        //EX.PatternColor = Color.FromArgb(54, 96, 145);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(250, 191, 143);
                        EX.Color = Color.FromArgb(250, 191, 143);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(253, 233, 217);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                        EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Color = Color.FromArgb(253, 233, 217);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Hair;
                    }

                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Double;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium8:
                    #region PivotStyleMedium8
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(128, 128, 128);
                        EX.Color = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(191, 191, 191);
                        EX.Color = Color.FromArgb(191, 191, 191);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.PatternColor = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(253, 233, 217);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }

                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium9:
                    #region PivotStyleMedium9
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(79, 129, 189);
                        EX.Color = Color.FromArgb(79, 129, 189);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(54, 96, 146);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(184, 204, 228);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(184, 204, 228);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(184, 204, 228);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(184, 204, 228);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(184, 204, 228);
                        EX.Color = Color.FromArgb(184, 204, 228);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.PatternColor = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.Color = Color.FromArgb(220, 230, 241);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(54, 96, 146);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(54, 96, 146);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium10:
                    #region PivotStyleMedium10
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(192, 80, 77);
                        EX.Color = Color.FromArgb(192, 80, 77);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(150,54 ,52);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Color = Color.FromArgb(230, 184, 183);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.PatternColor = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(150, 54, 52);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Hair;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(150, 54, 52);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium11:
                    #region PivotStyleMedium11
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(155, 187, 89);
                        EX.Color = Color.FromArgb(155, 187, 89);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(118, 147, 60);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(216, 228, 188);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(216, 228, 188);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(216, 228, 188);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(216, 228, 188);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(216, 228, 188);
                        EX.Color = Color.FromArgb(216, 228, 188);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.PatternColor = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(118, 147, 60);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Hair;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(118, 147, 60);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium12:
                    #region PivotStyleMedium12
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(128, 100, 162);
                        EX.Color = Color.FromArgb(128, 100, 162);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(96, 73, 122);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(204, 192, 218);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(204, 192, 218);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(204, 192, 218);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(204, 192, 218);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(204, 192, 218);
                        EX.Color = Color.FromArgb(204, 192, 218);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.PatternColor = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Color = Color.FromArgb(228, 223, 236);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(96, 73, 122);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Hair;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(96, 73, 122);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium13:
                    #region PivotStyleMedium13
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(75, 172, 198);
                        EX.Color = Color.FromArgb(75, 172, 198);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(49, 134, 155);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(183, 222, 232);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(183, 222, 232);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(183, 222, 232);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(183, 222, 232);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(183, 222, 232);
                        EX.Color = Color.FromArgb(183, 222, 232);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                        //EX.PatternColor = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Color = Color.FromArgb(218, 238, 243);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(49, 134, 155);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Hair;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(49, 134, 155);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium14:
                    #region PivotStyleMedium14
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(247, 150, 70);
                        EX.Color = Color.FromArgb(247, 150, 70);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thick;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(252, 213, 180);
                        EX.Color = Color.FromArgb(252, 213, 180);
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        //EX.PatternColor = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Color = Color.FromArgb(253, 233, 217);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Hair;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(226, 107, 10);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium15:
                    #region PivotStyleMedium15
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0); 
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(242, 242, 242);
                        EX.Color = Color.FromArgb(242, 242, 242);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(217, 217, 217);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(217, 217, 217);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(217, 217, 217);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(191, 191, 191);
                        EX.Color = Color.FromArgb(191, 191, 191);
                        //EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        // EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //  EX.PatternColor = Color.FromArgb(242, 242, 242);
                        EX.Font.Bold = true;
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        // EX.PatternColor = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        // EX.PatternColor = Color.FromArgb(0, 0, 0);   
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(128, 128, 128);
                        EX.Color = Color.FromArgb(128, 128, 128);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading3 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        // EX.Font.Bold = true;  
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(217, 217, 217);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.None;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium16:
                    #region PivotStyleMedium16
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.Color = Color.FromArgb(220, 230, 241);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(184, 204, 228);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(184, 204, 228);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(184, 204, 228);
                        EX.Color = Color.FromArgb(184, 204, 228);
                        //EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        // EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //    EX.PatternColor = Color.FromArgb(242, 242, 242);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        // EX.PatternColor = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        // EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(242, 242, 242);
                        EX.Color = Color.FromArgb(242, 242, 242);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading3 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        // EX.Font.Bold = true;  
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium17:
                    #region PivotStyleMedium17
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Color = Color.FromArgb(242, 220, 219);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Color = Color.FromArgb(230, 184, 183);
                        //   EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Font.Bold = true;
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        // EX.PatternColor = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);

                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        // EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Font.Bold = true;
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Color = Color.FromArgb(230, 184, 183);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading3 & tableParts) != 0)
                    {
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        // EX.Font.Bold = true;  
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium18:
                    #region PivotStyleMedium18
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0); 
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(216, 228, 188);
                        EX.Color = Color.FromArgb(216, 228, 188);
                        //   EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Font.Bold = true;
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        // EX.PatternColor = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        //EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Color = Color.FromArgb(230, 184, 183);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading3 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        // EX.Font.Bold = true;  
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium19:
                    #region PivotStyleMedium19
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Color = Color.FromArgb(228, 223, 236);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(204, 192, 218);
                        EX.Color = Color.FromArgb(204, 192, 218);
                        //   EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        // EX.PatternColor = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Color = Color.FromArgb(230, 184, 183);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading3 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        // EX.Font.Bold = true;  
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium20:
                    #region PivotStyleMedium20
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Color = Color.FromArgb(218, 238, 243);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(183, 222, 232);
                        EX.Color = Color.FromArgb(183, 222, 232);
                        //   EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        // EX.PatternColor = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Color = Color.FromArgb(230, 184, 183);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading3 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        // EX.Font.Bold = true;  
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium21:
                    #region PivotStyleMedium21
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Color = Color.FromArgb(253, 233, 217);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(252, 213, 180);
                        EX.Color = Color.FromArgb(252, 213, 180);
                        //   EX.Font.Bold = true;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(166, 166, 166);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        // EX.PatternColor = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        //EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Color = Color.FromArgb(230, 184, 183);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading3 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        // EX.Font.Bold = true;  
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium22:
                    #region PivotStyleMedium22
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Hair;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(191, 191, 191);
                        EX.Color = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        // EX.PatternColor = Color.FromArgb(0, 0, 0);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                        EX.Font.Bold = true;
                        // EX.PatternColor = Color.FromArgb(252, 213, 180);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(191, 191, 191);
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        //EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Font.Bold = true;
                    }                
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(128, 128, 128);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        // EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium23:
                    #region PivotStyleMedium23
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.Color = Color.FromArgb(220, 230, 241);
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(184, 204, 228);
                        EX.Color = Color.FromArgb(184, 204, 228);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.Color = Color.FromArgb(220, 230, 241);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.Color = Color.FromArgb(220, 230, 241);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        // EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium24:
                    #region PivotStyleMedium24
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.FontName = "Accent 2";
                        EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Color = Color.FromArgb(242, 220, 219);
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Color = Color.FromArgb(230, 184, 183);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Color = Color.FromArgb(242, 220, 219);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Color = Color.FromArgb(242, 220, 219);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        // EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        // EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium25:
                    #region PivotStyleMedium25
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(215, 228, 188);
                        EX.Color = Color.FromArgb(215, 228, 188);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        // EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium26:
                    #region PivotStyleMedium26
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.FontName = "Accent 2";
                        EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Color = Color.FromArgb(228, 223, 236);
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(204, 192, 218);
                        EX.Color = Color.FromArgb(204, 192, 218);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Color = Color.FromArgb(228, 223, 236);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Color = Color.FromArgb(228, 223, 236);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        // EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium27:
                    #region PivotStyleMedium27
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Color = Color.FromArgb(218, 238, 243);
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(183, 222, 232);
                        EX.Color = Color.FromArgb(183, 222, 232);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Color = Color.FromArgb(218, 238, 243);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Color = Color.FromArgb(218, 238, 243);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        // EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleMedium28:
                    #region PivotStyleMedium28
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Color = Color.FromArgb(253, 233, 217);
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(252, 213, 180);
                        EX.Color = Color.FromArgb(252, 213, 180);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Color = Color.FromArgb(253, 233, 217);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Color = Color.FromArgb(253, 233, 217);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        // EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark1:
                    #region PivotStyleDark1
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0,0,0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(191, 191, 191);
                        EX.Color = Color.FromArgb(191, 191, 191);
                        //With Insize Horizondal borders
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }                   
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(128, 128, 128);
                        EX.Color = Color.FromArgb(128, 128, 128);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        //Inside horizondal borders
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);  
                    }                   
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(166, 166, 166);

                        EX.PatternColor = Color.FromArgb(217,217,217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                       
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Color = Color.FromArgb(128,128,128);
                        EX.PatternColor = Color.FromArgb(128, 128, 128);
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(217, 217, 217);

                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark2:
                    #region PivotStyleDark2
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(184, 204, 228);
                        EX.Color = Color.FromArgb(184, 204, 228);
                        //With Insize Horizondal borders
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(36, 64, 98);
                        EX.Color = Color.FromArgb(36, 64, 98);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                        //Inside horizondal borders
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(149, 179, 215);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(149, 179, 215);

                        EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.Color = Color.FromArgb(220, 230, 241);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Color = Color.FromArgb(36, 64, 98);
                        EX.PatternColor = Color.FromArgb(36, 64, 98);
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(220, 230, 241);
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark3:
                    #region PivotStyleDark3 
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Color = Color.FromArgb(230, 184, 183);
                        //With Insize Horizondal borders
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(99, 37, 35);
                        EX.Color = Color.FromArgb(99, 37, 35);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                        //Inside horizondal borders
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(242, 220, 219);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 150, 148);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(218, 150, 148);

                        EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Color = Color.FromArgb(242, 220, 219);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Color = Color.FromArgb(99, 37, 35);
                        EX.PatternColor = Color.FromArgb(99, 37, 35);                     
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(242, 220, 219);
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark4:
                    #region PivotStyleDark4
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(216, 228, 188);
                        EX.Color = Color.FromArgb(216, 228, 188);
                        //With Insize Horizondal borders
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(79, 98, 40);
                        EX.Color = Color.FromArgb(79, 98, 40);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                        //Inside horizondal borders
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(235, 241, 222);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(196, 215, 155);

                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Color = Color.FromArgb(79, 98, 40);
                        EX.PatternColor = Color.FromArgb(79, 98, 40);
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(235, 241, 222);
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark5:
                    #region PivotStyleDark5
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(204, 192, 218);
                        EX.Color = Color.FromArgb(204, 192, 218);
                        //With Insize Horizondal borders
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(64, 49, 81);
                        EX.Color = Color.FromArgb(64, 49, 81);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                        //Inside horizondal borders
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(228, 223, 236);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(177, 160, 199);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(177, 160, 199);

                        EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Color = Color.FromArgb(228, 223, 236);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Color = Color.FromArgb(64, 49, 81);
                        EX.PatternColor = Color.FromArgb(64, 49, 81);
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(228, 223, 236);
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark6:
                    #region PivotStyleDark6
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(183, 222, 232);
                        EX.Color = Color.FromArgb(183, 222, 232);
                        //With Insize Horizondal borders
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(33, 89, 103);
                        EX.Color = Color.FromArgb(33, 89, 103);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                        //Inside horizondal borders
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 238, 243);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(146, 205, 220);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(146, 205, 220);

                        EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Color = Color.FromArgb(218, 238, 243);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Color = Color.FromArgb(33, 89, 103);
                        EX.PatternColor = Color.FromArgb(33, 89, 103);
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(218, 238, 243);
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark7:
                    #region PivotStyleDark7
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(252, 213, 180);
                        EX.Color = Color.FromArgb(252, 213, 180);
                        //With Insize Horizondal borders
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(151, 71, 6);
                        EX.Color = Color.FromArgb(151, 71, 6);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                        //Inside horizondal borders
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(253, 233, 217);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(250, 191, 143);
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(250, 191, 143);

                        EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Color = Color.FromArgb(253, 233, 217);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Color = Color.FromArgb(151, 71, 6);
                        EX.PatternColor = Color.FromArgb(151, 71, 6);
                        EX.Font.Bold = true;
                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(253, 233, 217);
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark8:
                    #region PivotStyleDark8
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0,0,0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);

                        EX.PatternColor = Color.FromArgb(217, 217, 217);
                        EX.Color = Color.FromArgb(217, 217, 217);
                    }                   
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(191, 191, 191);
                        EX.Color = Color.FromArgb(191, 191, 191);
                    }
                    if ((PivotTableParts.ColumnSubHeading2 & tableParts) != 0)
                    {
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(191, 191, 191);
                    }
                    if ((PivotTableParts.ColumnSubHeading3 & tableParts) != 0)
                    {
                        //EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(191, 191, 191);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(217, 217, 217);
                       
                        EX.PatternColor = Color.FromArgb(191,191,191);
                        EX.Color = Color.FromArgb(191, 191, 191);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                       
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark9:
                    #region PivotStyleDark9
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;                       
                        EX.PatternColor = Color.FromArgb(220, 230, 241);
                        EX.Color = Color.FromArgb(220, 230, 241);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(184, 204, 228);
                        EX.Color = Color.FromArgb(184, 204, 228);
                    }                  
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);

                        EX.PatternColor = Color.FromArgb(184, 204, 228);
                        EX.Color = Color.FromArgb(184, 204, 228);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark10:
                    #region PivotStyleDark10
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(242, 220, 219);
                        EX.Color = Color.FromArgb(242, 220, 219);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Color = Color.FromArgb(230, 184, 183);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(242, 220, 219);

                        EX.PatternColor = Color.FromArgb(230, 184, 183);
                        EX.Color = Color.FromArgb(230, 184, 183);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark11:
                    #region PivotStyleDark11
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(235, 241, 222);
                        EX.Color = Color.FromArgb(235, 241, 222);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(216, 228, 188);
                        EX.Color = Color.FromArgb(216, 228, 188);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(235, 241, 222);

                        EX.PatternColor = Color.FromArgb(216, 228, 188);
                        EX.Color = Color.FromArgb(216, 228, 188);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark12:
                    #region PivotStyleDark12
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(228, 223, 236);
                        EX.Color = Color.FromArgb(228, 223, 236);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(204, 192, 218);
                        EX.Color = Color.FromArgb(204, 192, 218);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(228, 223, 236);

                        EX.PatternColor = Color.FromArgb(204, 192, 218);
                        EX.Color = Color.FromArgb(204, 192, 218);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark13:
                    #region PivotStyleDark13
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(218, 238, 243);
                        EX.Color = Color.FromArgb(218, 238, 243);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(183, 222, 232);
                        EX.Color = Color.FromArgb(183, 222, 232);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 238, 243);

                        EX.PatternColor = Color.FromArgb(183, 222, 232);
                        EX.Color = Color.FromArgb(183, 222, 232);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark14:
                    #region PivotStyleDark14
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(253, 233, 217);
                        EX.Color = Color.FromArgb(253, 233, 217);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(252, 213, 180);
                        EX.Color = Color.FromArgb(252, 213, 180);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(253, 233, 217);

                        EX.PatternColor = Color.FromArgb(252, 213, 180);
                        EX.Color = Color.FromArgb(252, 213, 180);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark15:
                    #region PivotStyleDark15
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(217, 217, 217);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                
                        EX.PatternColor = Color.FromArgb(140, 140, 140);
                        EX.Color = Color.FromArgb(140, 140, 140);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(64, 64, 64);
                        EX.Color = Color.FromArgb(64, 64, 64);
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);

                        EX.PatternColor = Color.FromArgb(128, 128, 128);
                        EX.Color = Color.FromArgb(128, 128, 128);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);

                        EX.PatternColor = Color.FromArgb(128, 128, 128);
                        EX.Color = Color.FromArgb(128, 128, 128);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark16:
                    #region PivotStyleDark16
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(220, 230, 241);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(79, 129, 189);
                        EX.Color = Color.FromArgb(79, 129, 189);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(149, 179, 215);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(149, 179, 215);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(149, 179, 215);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(149, 179, 215);

                        EX.PatternColor = Color.FromArgb(54,96,146);
                        EX.Color = Color.FromArgb(54, 96, 146);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(149, 179, 215);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);

                        EX.PatternColor = Color.FromArgb(54, 96, 146);
                        EX.Color = Color.FromArgb(54, 96, 146);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark17:
                    #region PivotStyleDark17
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(242, 220, 219);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(192, 80, 77);
                        EX.Color = Color.FromArgb(192, 80, 77);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 150, 148);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 150, 148);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 150, 148);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 150, 148);

                        EX.PatternColor = Color.FromArgb(150, 54, 52);
                        EX.Color = Color.FromArgb(150, 54, 52);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 150, 148);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);

                        EX.PatternColor = Color.FromArgb(150, 54, 52);
                        EX.Color = Color.FromArgb(150, 54, 52);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark18:
                    #region PivotStyleDark18
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(235, 241, 222);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(155, 187, 89);
                        EX.Color = Color.FromArgb(155, 187, 89);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);

                        EX.PatternColor = Color.FromArgb(118, 147, 60);
                        EX.Color = Color.FromArgb(118, 147, 60);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);

                        EX.PatternColor = Color.FromArgb(118, 147, 60);
                        EX.Color = Color.FromArgb(118, 147, 60);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark19:
                    #region PivotStyleDark19
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(228, 223, 236);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(128, 100, 162);
                        EX.Color = Color.FromArgb(128, 100, 162);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);

                        EX.PatternColor = Color.FromArgb(96, 73, 122);
                        EX.Color = Color.FromArgb(96, 73, 122);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(96, 73, 122);
                        EX.Color = Color.FromArgb(96, 73, 122);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark20:
                    #region PivotStyleDark20
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(218, 238, 243);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(75, 172, 198);
                        EX.Color = Color.FromArgb(75, 172, 198);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(146, 205, 220);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(146, 205, 220);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(146, 205, 220);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(146, 205, 220);

                        EX.PatternColor = Color.FromArgb(49, 134, 155);
                        EX.Color = Color.FromArgb(49, 134, 155);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(146, 205, 220);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(49, 134, 155);
                        EX.Color = Color.FromArgb(49, 134, 155);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark21:
                    #region PivotStyleDark21
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(253, 233, 217);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(247, 150, 70);
                        EX.Color = Color.FromArgb(247, 150, 70);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                    }
                    if ((PivotTableParts.SubtotalColumn1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(250, 191, 143);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(250, 191, 143);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(250, 191, 143);
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(250, 191, 143);

                        EX.PatternColor = Color.FromArgb(226, 107, 10);
                        EX.Color = Color.FromArgb(226, 107, 10);
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.ColumnSubHeading1 & tableParts) != 0)
                    {
                        EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(250, 191, 143);
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.PatternColor = Color.FromArgb(226, 107, 10);
                        EX.Color = Color.FromArgb(226, 107, 10);
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.PatternColor = Color.FromArgb(0, 0, 0);
                        EX.Color = Color.FromArgb(0, 0, 0);
                        EX.Font.Bold = true;
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark22:
                    #region PivotStyleDark22
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(217, 217, 217);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(128, 128, 128);//Inside vetical border
                        EX.Color = Color.FromArgb(128, 128, 128);
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(128, 128, 128);
                        EX.Color = Color.FromArgb(128, 128, 128);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(128, 128, 128);
                        EX.Color = Color.FromArgb(128, 128, 128);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;                     
                    }                
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(217, 217, 217);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;                        
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark23:
                    #region PivotStyleDark23
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(220, 230, 241);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(79, 129, 189);//Inside vetical border
                        EX.Color = Color.FromArgb(79, 129, 189);
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(54, 96, 146);
                        EX.Color = Color.FromArgb(54, 96, 146);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(54, 96, 146);
                        EX.Color = Color.FromArgb(54, 96, 146);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(220, 230, 241);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark24:
                    #region PivotStyleDark24
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(242, 220, 219);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(192, 80, 77);//Inside vetical border
                        EX.Color = Color.FromArgb(192, 80, 77);
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(150, 54, 52);
                        EX.Color = Color.FromArgb(150, 54, 52);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(150, 54, 52);
                        EX.Color = Color.FromArgb(150, 54, 52);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(242, 220, 219);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark25:
                    #region PivotStyleDark25
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(235, 241, 222);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(155, 187, 89);//Inside vetical border
                        EX.Color = Color.FromArgb(155, 187, 89);
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(118, 147, 60);
                        EX.Color = Color.FromArgb(118, 147, 60);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(118, 147, 60);
                        EX.Color = Color.FromArgb(118, 147, 60);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(235, 241, 222);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark26:
                    #region PivotStyleDark26
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(228, 223, 236);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(128, 100, 162);//Inside vetical border
                        EX.Color = Color.FromArgb(128, 100, 162);
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(96, 73, 122);
                        EX.Color = Color.FromArgb(96, 73, 122);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(96, 73, 122);
                        EX.Color = Color.FromArgb(96, 73, 122);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(228, 223, 236);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark27:
                    #region PivotStyleDark27
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(218, 238, 243);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(75, 172, 198);//Inside vetical border
                        EX.Color = Color.FromArgb(75, 172, 198);
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(49, 134, 155);
                        EX.Color = Color.FromArgb(49, 134, 155);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(49, 134, 155);
                        EX.Color = Color.FromArgb(49, 134, 155);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(218, 238, 243);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    #endregion
                    break;
                case PivotBuiltInStyles.PivotStyleDark28:
                    #region PivotStyleDark28
                    if ((PivotTableParts.WholeTable & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(253, 233, 217);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.PatternColor = Color.FromArgb(247, 150, 70);//Inside vetical border
                        EX.Color = Color.FromArgb(247, 150, 70);
                        EX.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(255, 255, 255);
                        EX.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                    }
                    if ((PivotTableParts.FirstColumn & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(226, 107, 10);
                        EX.Color = Color.FromArgb(226, 107, 10);
                    }
                    if ((PivotTableParts.HeaderRow & tableParts) != 0)
                    {
                        EX.PatternColor = Color.FromArgb(226, 107, 10);
                        EX.Color = Color.FromArgb(226, 107, 10);

                        //EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        //EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    if ((PivotTableParts.FirstHeaderCell & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(253, 233, 217);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.SubtotalRow2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(0, 0, 0);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading1 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.RowSubHeading2 & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;

                        EX.Font.Bold = true;
                    }
                    if ((PivotTableParts.GrandTotalRow & tableParts) != 0)
                    {
                        EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                        EX.Font.FontName = "Calibri";
                        EX.Font.Size = 11;
                        EX.Font.Bold = true;

                        EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    #endregion
                    break;

            }
            return EX;
        }

        internal void DrawPivotBorder(PivotTableLayout layout, PivotBuiltInStyles? buildInStyles)
        {
            bool FirstSubtotalColumn1 = true;
            bool ChangeFirstSubtotalColumn = false;
            List<int> FirstSubtotalColumnIndex = new List<int>();

            switch (buildInStyles)
            {

                case PivotBuiltInStyles.PivotStyleLight1:
                    int lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight2:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(79, 129, 189);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight3:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(192, 80, 77);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight4:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(155, 187, 89);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight5:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 100, 162);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight6:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(75, 172, 198);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight7:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(247, 150, 70);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight8:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex + 1, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) == 0) && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                        
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight9:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(79, 129, 189);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(79, 129, 189);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(79, 129, 189);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(79, 129, 189);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex + 1, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) == 0) && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(79, 129, 189);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);

                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight10:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(192, 80, 77);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(192, 80, 77);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(192, 80, 77);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(192, 80, 77);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex + 1, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) == 0) && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(242, 220, 219);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(192, 80, 77);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);

                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight11:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(155, 187, 89);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(155, 187, 89);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(155, 187, 89);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(155, 187, 89);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex + 1, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) == 0) && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(235, 241, 222);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(155, 187, 89);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);

                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight12:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 100, 162);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 100, 162);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 100, 162);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 100, 162);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex + 1, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) == 0) && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(228, 223, 236);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 100, 162);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);

                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight13:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(75, 172, 198);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(75, 172, 198);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(75, 172, 198);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(75, 172, 198);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex + 1, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) == 0) && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 238, 243);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(75, 172, 198);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);

                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight14:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(247, 150, 70);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(247, 150, 70);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(247, 150, 70);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(247, 150, 70);
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex + 1, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) == 0) && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(253, 233, 217);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thick;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(247, 150, 70);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);

                    }
                    break;
                case PivotBuiltInStyles .PivotStyleLight15:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight16:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(149, 179, 215);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight17:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 150, 148);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight18:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight19:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(177, 160, 199);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight20:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(146, 205, 220);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight21:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(250, 191, 143);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight22:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }

                    break;
                case PivotBuiltInStyles.PivotStyleLight23:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(79, 129, 189);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(79, 129, 189);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(79, 129, 189);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight24:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(192, 80, 77);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(192, 80, 77);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(192, 80, 77);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight25:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(155, 187, 89);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(155, 187, 89);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(155, 187, 89);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight26:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 100, 162);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 100, 162);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 100, 162);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight27:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(75, 172, 198);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(75, 172, 198);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(75, 172, 198);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleLight28:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(247, 150, 70);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(247, 150, 70);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(247, 150, 70);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium1:
                    for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                    {
                        for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    if (colIndex != layout.maxColumnCount)
                                    {
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(217, 217, 217);
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium2:
                    for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                    {
                        for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                //if (((layout[rowIndex + 1, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) == 0) && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) == 0)
                                //    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                //    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                //{
                                //    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(184, 204, 228);
                                //    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Hair;
                                //}
                                if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    if (colIndex != layout.maxColumnCount)
                                    {
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(184, 204, 228);
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium3:
                    for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                    {
                        for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    if (colIndex != layout.maxColumnCount)
                                    {
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(230, 184, 183);
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium4:
                    for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                    {
                        for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    if (colIndex != layout.maxColumnCount)
                                    {
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(216, 228, 188);
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium5:
                    for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                    {
                        for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    if (colIndex != layout.maxColumnCount)
                                    {
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(204, 192, 218);
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium6:
                    for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                    {
                        for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    if (colIndex != layout.maxColumnCount)
                                    {
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(183, 222, 232);
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium7:
                    for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                    {
                        for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading1) != 0)
                            {
                                if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn2) == 0)
                                    && ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn3) == 0))
                                {
                                    if (colIndex != layout.maxColumnCount)
                                    {
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(252, 213, 180);
                                        layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                                    }
                                }
                            }
                        }
                    }

                    break;
               
                case PivotBuiltInStyles.PivotStyleMedium8:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                FirstSubtotalColumnIndex.Add(colIndex);
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(191, 191, 191);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(191, 191, 191);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(191, 191, 191);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (FirstSubtotalColumnIndex.Contains(colIndex))
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(191, 191, 191);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(191, 191, 191);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;

                case PivotBuiltInStyles.PivotStyleMedium9:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                FirstSubtotalColumnIndex.Add(colIndex);
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(184, 204, 228);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(184, 204, 228);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(184, 204, 228);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(54, 96, 146);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (FirstSubtotalColumnIndex.Contains(colIndex))
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(184, 204, 228);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(184, 204, 228);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium10:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                FirstSubtotalColumnIndex.Add(colIndex);
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(230, 184, 183);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(230, 184, 183);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(230, 184, 183);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(150, 54, 52);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (FirstSubtotalColumnIndex.Contains(colIndex))
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(230, 184, 183);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(230, 184, 183);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium11:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                FirstSubtotalColumnIndex.Add(colIndex);
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(216, 228, 188);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(216, 228, 188);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(216, 228, 188);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(118, 147, 60);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (FirstSubtotalColumnIndex.Contains(colIndex))
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(216, 228, 188);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(216, 228, 188);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium12:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                FirstSubtotalColumnIndex.Add(colIndex);
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(204, 192, 218);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(204, 192, 218);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(204, 192, 218);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(96, 73, 122);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (FirstSubtotalColumnIndex.Contains(colIndex))
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(204, 192, 218);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(204, 192, 218);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium13:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                FirstSubtotalColumnIndex.Add(colIndex);
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(183, 222, 232);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(183, 222, 232);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(183, 222, 232);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(49, 134, 155);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (FirstSubtotalColumnIndex.Contains(colIndex))
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(183, 222, 232);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(183, 222, 232);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium14:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                FirstSubtotalColumnIndex.Add(colIndex);
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(252, 213, 180);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(252, 213, 180);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(252, 213, 180);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(226, 107, 10);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (FirstSubtotalColumnIndex.Contains(colIndex))
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(252, 213, 180);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(252, 213, 180);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium15:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(165, 165, 165);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium16:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(165, 165, 165);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium17:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(165, 165, 165);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium18:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(165, 165, 165);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium19:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(165, 165, 165);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium20:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(165, 165, 165);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium21:
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.SubtotalColumn1) != 0)
                            {
                                if (FirstSubtotalColumn1)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(165, 165, 165);
                                    ChangeFirstSubtotalColumn = true;
                                }
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(165, 165, 165);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Thin;
                            }
                        }
                        if (ChangeFirstSubtotalColumn)
                            FirstSubtotalColumn1 = false;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium22:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium23:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium24:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium25:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium26:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium27:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleMedium28:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark8:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;                                
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading2) != 0)
                                    || ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading3) != 0))
                            {

                                if (layout[rowIndex, colIndex].Value != "")
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(191, 191, 191);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);                      
                    }                    
                    break;
                case PivotBuiltInStyles.PivotStyleDark9:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;                               
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading2) != 0)
                                    || ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading3) != 0))
                            {

                                if (layout[rowIndex, colIndex].Value != "")
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(220, 230, 241);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                        //layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                        //layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark10:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading2) != 0)
                                    || ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading3) != 0))
                            {

                                if (layout[rowIndex, colIndex].Value != "")
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(242, 220, 219);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark11:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading2) != 0)
                                    || ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading3) != 0))
                            {

                                if (layout[rowIndex, colIndex].Value != "")
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(235, 242, 222);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark12:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading2) != 0)
                                    || ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading3) != 0))
                            {

                                if (layout[rowIndex, colIndex].Value != "")
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(228, 223, 236);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark13:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading2) != 0)
                                    || ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading3) != 0))
                            {

                                if (layout[rowIndex, colIndex].Value != "")
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(218, 238, 243);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark14:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.None;
                            }
                            if (colIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == 0)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                            }
                            if (rowIndex == layout.maxRowCount)
                            {
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                                layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                            }
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.GrandTotalRow) != 0)
                            {
                                if (colIndex == 0)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                                if (colIndex == layout.maxColumnCount)
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                                }
                            }
                            if (colIndex == layout.maxColumnCount)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeRight].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            else if (colIndex == 0)
                            {
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].LineStyle = ExcelLineStyle.Medium;
                                layout[lastHeaderRow, colIndex].XF.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB = Color.FromArgb(128, 128, 128);
                            }
                            if (((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading2) != 0)
                                    || ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.ColumnSubHeading3) != 0))
                            {

                                if (layout[rowIndex, colIndex].Value != "")
                                {
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(253, 233, 217);
                                    layout[rowIndex, colIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                                }
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Medium;
                        layout[0, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark22:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }                            
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark23:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark24:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark25:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark26:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark27:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                    }
                    break;
                case PivotBuiltInStyles.PivotStyleDark28:
                    lastHeaderRow = 0;
                    for (int rowIndex = 0, rowCount = layout.maxRowCount; rowIndex <= rowCount; rowIndex++)
                    {
                        for (int colIndex = 0, colCount = layout.maxColumnCount; colIndex <= colCount; colIndex++)
                        {
                            if ((layout[rowIndex, colIndex].PivotTablePartStyle & PivotTableParts.HeaderRow) != 0)
                            {
                                if (rowIndex > lastHeaderRow)
                                    lastHeaderRow = rowIndex;
                            }
                        }
                    }
                    for (int colHeaderIndex = 0; colHeaderIndex <= layout.maxColumnCount; colHeaderIndex++)
                    {
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(255, 255, 255);
                        layout[lastHeaderRow, colHeaderIndex].XF.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Medium;
                    }
                    break;

            }
        }
        internal ExtendedFormatImpl GetPageFilterLabel(PivotBuiltInStyles? buildInStyles)
        {
            ExtendedFormatImpl EX = new ExtendedFormatImpl(wkSheet.Application, wkSheet.Workbook);
            WorkbookImpl m_book = wkSheet.Workbook as WorkbookImpl;
            FontImpl font = (FontImpl)m_book.CreateFont(null, false);

            font = (FontImpl)m_book.InnerFonts.Add(font);
            EX.FontIndex = font.Font.Index;

            switch (buildInStyles)
            {
                case PivotBuiltInStyles.PivotStyleLight1:
                    #region PivotStylesLight1
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight2:
                    #region PivotStylesLight2
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(79, 129, 189);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight3:
                    #region PivotStylesLight3
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(192, 80, 77);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight4:
                    #region PivotStylesLight4
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(155, 187, 89);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight5:
                    #region PivotStylesLight5
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 100, 162);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight6:
                    #region PivotStylesLight6
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(75, 172, 198);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight7:
                    #region PivotStylesLight7
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(247, 150, 70);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight8:
                    #region PivotStylesLight8
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight9:
                    #region PivotStylesLight9
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight10:
                    #region PivotStylesLight10
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight11:
                    #region PivotStylesLight11
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight12:
                    #region PivotStylesLight12
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight13:
                    #region PivotStylesLight13
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight14:
                    #region PivotStylesLight14
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight15:
                    #region PivotStylesLight15
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Color = Color.FromArgb(217, 217, 217);
                    EX.PatternColor = Color.FromArgb(217, 217, 217);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight16:
                    #region PivotStylesLight16
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(149, 179, 215);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Color = Color.FromArgb(220, 230, 241);
                    EX.PatternColor = Color.FromArgb(220, 230, 241);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight17:
                    #region PivotStylesLight17
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 150, 148);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Color = Color.FromArgb(242, 220, 219);
                    EX.PatternColor = Color.FromArgb(242, 220, 219);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight18:
                    #region PivotStylesLight18
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Color = Color.FromArgb(235, 241, 222);
                    EX.PatternColor = Color.FromArgb(235, 241, 222);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight19:
                    #region PivotStylesLight19
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(177, 160, 199);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Color = Color.FromArgb(228, 223, 236);
                    EX.PatternColor = Color.FromArgb(228, 223, 236);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight20:
                    #region PivotStylesLight20
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 238, 243);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Color = Color.FromArgb(146, 205, 220);
                    EX.PatternColor = Color.FromArgb(146, 205, 220);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight21:
                    #region PivotStylesLight21
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(250, 191, 143);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.PatternColor = Color.FromArgb(253, 233, 217);
                    EX.Color = Color.FromArgb(253, 233, 217);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight22:
                    #region PivotStylesLight22
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(0,0,0);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight23:
                    #region PivotStylesLight23
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight24:
                    #region PivotStylesLight24
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight25:
                    #region PivotStylesLight25
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight26:
                    #region PivotStylesLight26
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight27:
                    #region PivotStylesLight27
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight28:
                    #region PivotStylesLight28
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium1:
                    #region PivotStylesMedium1
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(217, 217, 217);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(217, 217, 217);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium2:
                    #region PivotStylesMedium2
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(220, 230, 241);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium3:
                    #region PivotStylesMedium3
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(242, 220, 219);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(242, 220, 219);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium4:
                    #region PivotStylesMedium4
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(235, 241, 222);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(235, 241, 222);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium5:
                    #region PivotStylesMedium5
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(228, 223, 236);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(228, 223, 236);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium6:
                    #region PivotStylesMedium6
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 238, 243);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(218, 238, 243);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium7:
                    #region PivotStylesMedium7
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(253, 233, 217);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(253, 233, 217);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium8:
                    #region PivotStylesMedium8
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Color = Color.FromArgb(217, 217, 217);
                    EX.PatternColor = Color.FromArgb(217, 217, 217);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium9:
                    #region PivotStylesMedium9
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(220, 230, 241);
                    EX.Color = Color.FromArgb(220, 230, 241);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium10:
                    #region PivotStylesMedium10
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(242, 220, 219);
                    EX.Color = Color.FromArgb(242, 220, 219);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium11:
                    #region PivotStylesMedium11
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(235, 241, 222);
                    EX.Color = Color.FromArgb(235, 241, 222);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium12:
                    #region PivotStylesMedium12
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(228, 223, 236);
                    EX.Color = Color.FromArgb(228, 223, 236);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium13:
                    #region PivotStylesMedium13
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(218, 238, 243);
                    EX.Color = Color.FromArgb(218, 238, 243);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium14:
                    #region PivotStylesMedium14
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(253, 233, 217);
                    EX.Color = Color.FromArgb(253, 233, 217);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium15:
                    #region PivotStylesMedium15
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(217, 217, 217);
                    EX.Color = Color.FromArgb(217, 217, 217);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium16:
                    #region PivotStylesMedium16
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(220, 230, 241);
                    EX.Color = Color.FromArgb(220, 230, 241);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium17:
                    #region PivotStylesMedium17
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(242, 220, 219);
                    EX.Color = Color.FromArgb(242, 220, 219);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium18:
                    #region PivotStylesMedium18
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(235, 241, 222);
                    EX.Color = Color.FromArgb(235, 241, 222);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium19:
                    #region PivotStylesMedium19
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(228, 223, 236);
                    EX.Color = Color.FromArgb(228, 223, 236);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium20:
                    #region PivotStylesMedium20
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(218, 238, 243);
                    EX.Color = Color.FromArgb(218, 238, 243);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium21:
                    #region PivotStylesMedium21
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(253, 233, 217);
                    EX.Color = Color.FromArgb(253, 233, 217);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium22:
                    #region PivotStylesMedium22
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(191, 191, 191);
                    EX.Color = Color.FromArgb(191, 191, 191);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium23:
                    #region PivotStylesMedium23
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(184, 204, 228);
                    EX.Color = Color.FromArgb(184, 204, 228);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium24:
                    #region PivotStylesMedium24
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(230, 184, 183);
                    EX.Color = Color.FromArgb(230, 184, 183);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium25:
                    #region PivotStylesMedium25
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(215, 228, 188);
                    EX.Color = Color.FromArgb(215, 228, 188);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium26:
                    #region PivotStylesMedium26
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Color = Color.FromArgb(204, 192, 218);
                    EX.PatternColor = Color.FromArgb(204, 192, 218);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium27:
                    #region PivotStylesMedium27
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(183, 222, 232);
                    EX.Color = Color.FromArgb(183, 222, 232);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium28:
                    #region PivotStylesMedium28
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(252, 213, 180);
                    EX.Color = Color.FromArgb(252, 213, 180);                    
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark1:
                    #region PivotStylesDark1
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(128, 128, 128);
                    EX.Color = Color.FromArgb(128, 128, 128);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark2:
                    #region PivotStylesDark2
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(36, 64, 98);
                    EX.Color = Color.FromArgb(36, 64, 98);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark3:
                    #region PivotStylesDark3
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(99, 37, 35);
                    EX.Color = Color.FromArgb(99, 37, 35);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark4:
                    #region PivotStylesDark4
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(79, 98, 40);
                    EX.Color = Color.FromArgb(79, 98, 40);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark5:
                    #region PivotStylesDark5
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(64, 49, 81);
                    EX.Color = Color.FromArgb(64, 49, 81);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark6:
                    #region PivotStylesDark6
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(33, 89, 103);
                    EX.Color = Color.FromArgb(33, 89, 103);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark7:
                    #region PivotStylesDark7
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(151, 71, 6);
                    EX.Color = Color.FromArgb(151, 71, 6);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark8:                    
                    #region PivotStylesDark8
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(217, 217, 217);
                    EX.Color = Color.FromArgb(217, 217, 217);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark9:
                    #region PivotStylesDark9
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(220, 230, 241);
                    EX.Color = Color.FromArgb(220, 230, 241);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark10:
                    #region PivotStylesDark10
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(242, 220, 219);
                    EX.Color = Color.FromArgb(242, 220, 219);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark11:
                    #region PivotStylesDark11
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(235, 241, 222);
                    EX.Color = Color.FromArgb(235, 241, 222);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark12:
                    #region PivotStylesDark12
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(228, 223, 236);
                    EX.Color = Color.FromArgb(228, 223, 236);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion

                case PivotBuiltInStyles.PivotStyleDark13:
                    #region PivotStylesDark13
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(218, 238, 243);
                    EX.Color = Color.FromArgb(218, 238, 243);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark14:
                    #region PivotStylesDark14
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(253, 233, 217);
                    EX.Color = Color.FromArgb(253, 233, 217);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark15:
                    #region PivotStylesDark15
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(140, 140, 140);
                    EX.Color = Color.FromArgb(140, 140, 140);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark16:
                    #region PivotStylesDark16
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(79, 129, 189);
                    EX.Color = Color.FromArgb(79, 129, 189);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark17:
                    #region PivotStylesDark17
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(192, 80, 77);
                    EX.Color = Color.FromArgb(192, 80, 77);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark18:
                    #region PivotStylesDark18
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(155, 187, 89);
                    EX.Color = Color.FromArgb(155, 187, 89);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark19:
                    #region PivotStylesDark19
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(128, 100, 162);
                    EX.Color = Color.FromArgb(128, 100, 162);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark20:
                    #region PivotStylesDark20
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(75, 172, 198);
                    EX.Color = Color.FromArgb(75, 172, 198);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark21:
                    #region PivotStylesDark21
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(247, 150, 70);
                    EX.Color = Color.FromArgb(247, 150, 70);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark22:
                    #region PivotStylesDark22
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(128, 128, 128);
                    EX.Color = Color.FromArgb(128, 128, 128);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark23:
                    #region PivotStylesDark23
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(54, 96, 146);
                    EX.Color = Color.FromArgb(54, 96, 146);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark24:
                    #region PivotStylesDark24
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(150, 54, 52);
                    EX.Color = Color.FromArgb(150, 54, 52);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark25:
                    #region PivotStylesDark25
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(118, 147, 60);
                    EX.Color = Color.FromArgb(118, 147, 60);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark26:
                    #region PivotStylesDark26
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(96, 73, 122);
                    EX.Color = Color.FromArgb(96, 73, 122);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark27:
                    #region PivotStylesDark27
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(49, 134, 155);
                    EX.Color = Color.FromArgb(49, 134, 155);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark28:
                    #region PivotStylesDark28
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Color = Color.FromArgb(226, 107, 10);
                    EX.PatternColor = Color.FromArgb(226, 107, 10);
                    EX.Color = Color.FromArgb(226, 107, 10);
                    break;
                    #endregion
            }
            return EX;
        }

        internal ExtendedFormatImpl GetPageFilterValue(PivotBuiltInStyles? buildInStyles)
        {
            ExtendedFormatImpl EX = new ExtendedFormatImpl(wkSheet.Application, wkSheet.Workbook);
            WorkbookImpl m_book = wkSheet.Workbook as WorkbookImpl;
            FontImpl font = (FontImpl)m_book.CreateFont(null, false);

            font = (FontImpl)m_book.InnerFonts.Add(font);
            EX.FontIndex = font.Font.Index;

            switch (buildInStyles)
            {
                case PivotBuiltInStyles.PivotStyleLight1:
                    #region PivotStylesLight1
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight2:
                    #region PivotStylesLight2
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(79, 129, 189);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(79, 129, 189);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight3:
                    #region PivotStylesLight3
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(192, 80, 77);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(192, 80, 77);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight4:
                    #region PivotStylesLight4
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(155, 187, 89);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(155, 187, 89);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight5:
                    #region PivotStylesLight5
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 100, 162);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 100, 162);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight6:
                    #region PivotStylesLight6
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(75, 172, 198);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(75, 172, 198);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight7:
                    #region PivotStylesLight7
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(247, 150, 70);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(247, 150, 70);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight8:
                    #region PivotStylesLight8
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight9:
                    #region PivotStylesLight9
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight10:
                    #region PivotStylesLight10
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight11:
                    #region PivotStylesLight11
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight12:
                    #region PivotStylesLight12
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight13:
                    #region PivotStylesLight13
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(49, 134, 155);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight14:
                    #region PivotStylesLight14
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight15:
                    #region PivotStylesLight15
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(166, 166, 166);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.PatternColor = Color.FromArgb(217, 217, 217);
                    EX.Color = Color.FromArgb(217, 217, 217);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight16:
                    #region PivotStylesLight16
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(149, 179, 215);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.PatternColor = Color.FromArgb(220, 230, 241);
                    EX.Color = Color.FromArgb(220, 230, 241);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight17:
                    #region PivotStylesLight17
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 150, 148);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.PatternColor = Color.FromArgb(242, 220, 219);
                    EX.Color = Color.FromArgb(242, 220, 219);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight18:
                    #region PivotStylesLight18
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(196, 215, 155);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.PatternColor = Color.FromArgb(235, 241, 222);
                    EX.Color = Color.FromArgb(235, 241, 222);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight19:
                    #region PivotStylesLight19
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(177, 160, 199);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.PatternColor = Color.FromArgb(228, 223, 236);
                    EX.Color = Color.FromArgb(228, 223, 236);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight20:
                    #region PivotStylesLight20
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 238, 243);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.PatternColor = Color.FromArgb(146, 205, 220);
                    EX.Color = Color.FromArgb(146, 205, 220);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight21:
                    #region PivotStylesLight21
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(250, 191, 143);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.PatternColor = Color.FromArgb(253, 233, 217);
                    EX.Color = Color.FromArgb(253, 233, 217);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight22:
                    #region PivotStylesLight22
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight23:
                    #region PivotStylesLight23
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(54, 96, 146);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight24:
                    #region PivotStylesLight24
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(150, 54, 52);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight25:
                    #region PivotStylesLight25
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(118, 147, 60);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight26:
                    #region PivotStylesLight26
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight27:
                    #region PivotStylesLight27
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(96, 73, 122);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleLight28:
                    #region PivotStylesLight28
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(226, 107, 10);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium1:
                    #region PivotStylesMedium1
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(217, 217, 217);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(217, 217, 217);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium2:
                    #region PivotStylesMedium2
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(220, 230, 241);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(220, 230, 241);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium3:
                    #region PivotStylesMedium3
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(242, 220, 219);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(242, 220, 219);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium4:
                    #region PivotStylesMedium4
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(235, 241, 222);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(235, 241, 222);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium5:
                    #region PivotStylesMedium5
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(228, 223, 236);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(228, 223, 236);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium6:
                    #region PivotStylesMedium6
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(218, 238, 243);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(218, 238, 243);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium7:
                    #region PivotStylesMedium7
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(253, 233, 217);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(253, 233, 217);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium8:
                    #region PivotStylesMedium8
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(217, 217, 217);
                    EX.Color = Color.FromArgb(217, 217, 217);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium9:
                    #region PivotStylesMedium9
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(220, 230, 241);
                    EX.Color = Color.FromArgb(220, 230, 241);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium10:
                    #region PivotStylesMedium10
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(242, 220, 219);
                    EX.Color = Color.FromArgb(242, 220, 219);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium11:
                    #region PivotStylesMedium11
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(235, 241, 222);
                    EX.Color = Color.FromArgb(235, 241, 222);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium12:
                    #region PivotStylesMedium12
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(228, 223, 236);
                    EX.Color = Color.FromArgb(228, 223, 236);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium13:
                    #region PivotStylesMedium13
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(218, 238, 243);
                    EX.Color = Color.FromArgb(218, 238, 243);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium14:
                    #region PivotStylesMedium14
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(253, 233, 217);
                    EX.Color = Color.FromArgb(253, 233, 217);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium15:
                    #region PivotStylesMedium15
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(217, 217, 217);
                    EX.Color = Color.FromArgb(217, 217, 217);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium16:
                    #region PivotStylesMedium16
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(220, 230, 241);
                    EX.Color = Color.FromArgb(220, 230, 241);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium17:
                    #region PivotStylesMedium17
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(242, 220, 219);
                    EX.Color = Color.FromArgb(242, 220, 219);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium18:
                    #region PivotStylesMedium18
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(235, 241, 222);
                    EX.Color = Color.FromArgb(235, 241, 222);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium19:
                    #region PivotStylesMedium19
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(228, 223, 236);
                    EX.Color = Color.FromArgb(228, 223, 236);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium20:
                    #region PivotStylesMedium20
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(218, 238, 243);
                    EX.Color = Color.FromArgb(218, 238, 243);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium21:
                    #region PivotStylesMedium21
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(253, 233, 217);
                    EX.Color = Color.FromArgb(253, 233, 217);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(0, 0, 0);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium22:
                    #region PivotStylesMedium22
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(217, 217, 217);
                    EX.Color = Color.FromArgb(217, 217, 217);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium23:
                    #region PivotStylesMedium23
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(220, 230, 241);
                    EX.Color = Color.FromArgb(220, 230, 241);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium24:
                    #region PivotStylesMedium23
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(242, 220, 219);
                    EX.Color = Color.FromArgb(242, 220, 219);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium25:
                    #region PivotStylesMedium23
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(235, 241, 222);
                    EX.Color = Color.FromArgb(235, 241, 222);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium26:
                    #region PivotStylesMedium23
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(228, 223, 236);
                    EX.Color = Color.FromArgb(228, 223, 236);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium27:
                    #region PivotStylesMedium23
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(218, 238, 243);
                    EX.Color = Color.FromArgb(218, 238, 243);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleMedium28:
                    #region PivotStylesMedium23
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(253, 233, 217);
                    EX.Color = Color.FromArgb(253, 233, 217);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark1:
                    #region PivotStylesDark1
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(128, 128, 128);
                    EX.Color = Color.FromArgb(128, 128, 128);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark2:
                    #region PivotStylesDark2
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;                    
                    EX.PatternColor = Color.FromArgb(36, 64, 98);
                    EX.Color = Color.FromArgb(36, 64, 98);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark3:
                    #region PivotStylesDark3
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(99, 37, 35);
                    EX.Color = Color.FromArgb(99, 37, 35);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark4:
                    #region PivotStylesDark4
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(79, 98, 40);
                    EX.Color = Color.FromArgb(79, 98, 40);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark5:
                    #region PivotStylesDark5
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(64, 49, 81);
                    EX.Color = Color.FromArgb(64, 49, 81);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark6:
                    #region PivotStylesDark6
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(33, 89, 103);
                    EX.Color = Color.FromArgb(33, 89, 103);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark7:
                    #region PivotStylesDark7
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(151, 71, 6);
                    EX.Color = Color.FromArgb(151, 71, 6);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark8:
                    #region PivotStylesDark8
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(217, 217, 217);
                    EX.Color = Color.FromArgb(217, 217, 217);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark9:
                    #region PivotStylesDark9
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(220, 230, 241);
                    EX.Color = Color.FromArgb(220, 230, 241);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark10:
                    #region PivotStylesDark10
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(242, 220, 219);
                    EX.Color = Color.FromArgb(242, 220, 219);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark11:
                    #region PivotStylesDark11
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(235, 241, 222);
                    EX.Color = Color.FromArgb(235, 241, 222);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark12:
                    #region PivotStylesDark12
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(228, 223, 236);
                    EX.Color = Color.FromArgb(228, 223, 236);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark13:
                    #region PivotStylesDark13
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(218, 238, 243);
                    EX.Color = Color.FromArgb(218, 238, 243);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark14:
                    #region PivotStylesDark14
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(253, 233, 217);
                    EX.Color = Color.FromArgb(253, 233, 217);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                    EX.Borders[ExcelBordersIndex.EdgeTop].ColorRGB = Color.FromArgb(128, 128, 128);
                    EX.Borders[ExcelBordersIndex.EdgeTop].LineStyle = ExcelLineStyle.Thin;
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark15:
                    #region PivotStylesDark15
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(140, 140, 140);
                    EX.Color = Color.FromArgb(140, 140, 140);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark16:
                    #region PivotStylesDark16
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(79, 129, 189);
                    EX.Color = Color.FromArgb(79, 129, 189);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark17:
                    #region PivotStylesDark17
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(192, 80, 77);
                    EX.Color = Color.FromArgb(192, 80, 77);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark18:
                    #region PivotStylesDark18
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(155, 187, 89);
                    EX.Color = Color.FromArgb(155, 187, 89);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark19:
                    #region PivotStylesDark19
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(128, 100, 162);
                    EX.Color = Color.FromArgb(128, 100, 162);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark20:
                    #region PivotStylesDark20
                    EX.PatternColor = Color.FromArgb(75, 172, 198);
                    EX.Color = Color.FromArgb(75, 172, 198);
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark21:
                    #region PivotStylesDark21
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(247, 150, 70);
                    EX.Color = Color.FromArgb(247, 150, 70);
                    EX.Font.RGBColor = Color.FromArgb(255, 255, 255);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark22:
                    #region PivotStylesDark22
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(128, 128, 128);
                    EX.Color = Color.FromArgb(128, 128, 128);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark23:
                  #region PivotStylesDark23
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(79, 129, 189);
                    EX.Color = Color.FromArgb(79, 129, 189);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark24:
                    #region PivotStylesDark24
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Color = Color.FromArgb(192, 80, 77);
                    EX.PatternColor = Color.FromArgb(192, 80, 77);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark25:
                    #region PivotStylesDark25
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.Color = Color.FromArgb(155, 187, 89);
                    EX.PatternColor = Color.FromArgb(155, 187, 89);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark26:
                    #region PivotStylesDark25
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(128, 100, 162);
                    EX.Color = Color.FromArgb(128, 100, 162);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark27:
                    #region PivotStylesDark27
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(75, 172, 198);
                    EX.Color = Color.FromArgb(75, 172, 198);
                    break;
                    #endregion
                case PivotBuiltInStyles.PivotStyleDark28:
                    #region PivotStylesDark28
                    EX.Font.FontName = "Calibri";
                    EX.Font.Size = 11;
                    EX.PatternColor = Color.FromArgb(247, 150, 70);
                    EX.Color = Color.FromArgb(247, 150, 70);
                    break;
                    #endregion
            }
            return EX;
        }
    }
}
#endif