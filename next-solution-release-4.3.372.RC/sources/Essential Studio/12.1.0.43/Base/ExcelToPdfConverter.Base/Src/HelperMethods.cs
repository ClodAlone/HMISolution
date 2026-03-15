#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Pdf.Graphics;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.ExcelToPdfConverter
{
    internal class HelperMethods
    {
        private PdfUnitConvertor unitConverter = new PdfUnitConvertor();
        private PageSetupOption pageSetupOption;
        /// <summary>
        /// Represents row title value.
        /// </summary>
        internal float rowTitle = 0.0f;
        /// <summary>
        /// Represents column title value.
        /// </summary>
        internal float colTitle = 0.0f;
        /// <summary>
        /// Represents font value.
        /// </summary>
        internal double[] arrayValue = new double[] { 0, 0 };

        internal HelperMethods(PageSetupOption PageSetUp)
        {
            pageSetupOption = PageSetUp;
        }
        internal int[] RowBreaker(int sheetFirstRow, int sheetLastRow, float sheetHeight, ItemSizeHelper RowHeightGetter, out int rowStartIndex,ExcelToPdfConverterSettings settings)
        {
            int[] result = new int[1048576];
            result[0] = sheetFirstRow;
            rowStartIndex = 1;
            float heightLimit = 0;
            bool checkSetupOption = false;
            bool IsCustomScaling = (settings.LayoutOptions == LayoutOptions.CustomScaling && (!pageSetupOption.PageSetup.IsFitToPage));

            if (IsCustomScaling)
            {
                arrayValue = CalculateFontValue(settings);
                PrintTitlesHeight();
            }
            
            for (int i = sheetFirstRow; i <= sheetLastRow; i++)
            {
                if(IsCustomScaling)
                    heightLimit += ((float)pageSetupOption.Worksheet.GetRowHeight(i)) * (float)arrayValue[1];
                else
                    heightLimit += unitConverter.ConvertFromPixels(RowHeightGetter.GetSize(i), PdfGraphicsUnit.Point);

                if (heightLimit >= sheetHeight)
                {
                    checkSetupOption = false;
                    result[rowStartIndex] = i - 1;                    
                    rowStartIndex++;
                    heightLimit = 0;
                    i--;                    
                }

                if (!pageSetupOption.CheckRowBounds(result[rowStartIndex - 1], sheetLastRow) && pageSetupOption.HasPrintTitleRows)
                    {
                        if (!checkSetupOption && sheetLastRow > pageSetupOption.PrintTitleLastRow)
                        {
                            if (IsCustomScaling)
                                heightLimit += rowTitle;
                            else
                                heightLimit += pageSetupOption.TitleRowHeight;
                            pageSetupOption.RowIndexes.Add(rowStartIndex);
                            checkSetupOption = true;
                        }
                }
            }
            return result;
        }
        internal int[] ColumnBreaker(int sheetFirstColumn, int sheetLastColumn, float sheetWidth, ItemSizeHelper ColumnWidthGetter, out int columnStartIndex,ExcelToPdfConverterSettings settings)
        {            
            int[] result = new int[16384];
            float widthLimit = 0;            
            result[0] = sheetFirstColumn;
            columnStartIndex = 1;
            bool checkSetupOption = false;
            bool IsCustomScaling = (settings.LayoutOptions == LayoutOptions.CustomScaling && (!pageSetupOption.PageSetup.IsFitToPage));

            if (IsCustomScaling)
            {
                arrayValue = CalculateFontValue(settings);
                PrintTitlesWidth(ColumnWidthGetter);
            }

            for (int i = sheetFirstColumn, j = 0; i <= sheetLastColumn; i++)
            {
                if (IsCustomScaling)
                    widthLimit += unitConverter.ConvertFromPixels(ColumnWidthGetter.GetSize(i), PdfGraphicsUnit.Point) * (float)arrayValue[0];
                else
                    widthLimit += unitConverter.ConvertFromPixels(ColumnWidthGetter.GetSize(i), PdfGraphicsUnit.Point);

                if (widthLimit >= sheetWidth)
                {
                    if (IsCustomScaling)
                        widthLimit -= unitConverter.ConvertFromPixels(ColumnWidthGetter.GetSize(i),
                                                                                     PdfGraphicsUnit.Point) * (float)arrayValue[0];
                    else
                        widthLimit -= unitConverter.ConvertFromPixels(ColumnWidthGetter.GetSize(i),
                                                                                     PdfGraphicsUnit.Point);
                    if ((int)widthLimit == 0)
                    {
                        result[columnStartIndex] = i;
                        i++;
                    }
                    else
                    {
                        result[columnStartIndex] = i - 1;
                        i--;
                    }
                    checkSetupOption = false;
                    columnStartIndex++;
                    widthLimit = 0;
                    j++;                      
                }

                if (pageSetupOption.HasPrintTitleColumns && !pageSetupOption.CheckColumnBounds(result[columnStartIndex - 1], sheetLastColumn))
                {
                    if (!checkSetupOption)
                    {
                        if (IsCustomScaling)
                            widthLimit += colTitle;
                        else
                            widthLimit += pageSetupOption.TitleColumnWidth;
                        pageSetupOption.ColumnIndexes.Add(columnStartIndex);
                        checkSetupOption = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get print title width for custom scaling.
        /// </summary>
        private void PrintTitlesWidth(ItemSizeHelper ColumnWidthGetter)
        {
            if (pageSetupOption.HasPrintTitleColumns)
            {
                colTitle = 0.0f;
                for (int i = pageSetupOption.PrintTitleFirstColumn; i <= pageSetupOption.PrintTitleLastColumn; i++)
                {
                    colTitle += unitConverter.ConvertFromPixels(ColumnWidthGetter.GetSize(i), PdfGraphicsUnit.Point) * (float)arrayValue[0];
                }
            }           
        }

        /// <summary>
        /// Get print title Height for custom scaling.
        /// </summary>
        private void PrintTitlesHeight()
        {
            if (pageSetupOption.HasPrintTitleRows)
            {
                rowTitle = 0.0f;
                for (int i = pageSetupOption.PrintTitleFirstRow; i <= pageSetupOption.PrintTitleLastRow; i++)
                {
                    rowTitle += ((float)pageSetupOption.Worksheet.GetRowHeight(i)) * (float)arrayValue[1];
                }
            }
        }

        /// <summary>
        /// Calculate the font style value.
        /// </summary>
        internal double[] CalculateFontValue(ExcelToPdfConverterSettings settings)
        {
            double[] array1 = new double[] { 1.0, 1.0 };
            double[] array2 = GetFontStyleValue(pageSetupOption.Worksheet);

            double[] result = new double[] { 0.0, 0.0 };

            double zoomValue = ((double)pageSetupOption.Worksheet.PageSetup.Zoom) / 100.0;

            if (settings.LayoutOptions == LayoutOptions.NoScaling )
                zoomValue = 1.00;

            result = new double[] { (array1[0] * array2[0]) * zoomValue, (array1[1] * array2[1]) * zoomValue };

            return result;
        }

        /// <summary>
        /// Get the font style value.
        /// </summary>
        private double[] GetFontStyleValue(IWorksheet sheet)
        {
            IStyle style = sheet.Workbook.Styles["Normal"];
            IFont m_font = style.Font;

            double var1 = 1.0;
            double var2 = 1.0;
            if (m_font.FontName == "Times New Roman")
            {
                switch ((int)m_font.Size)
                {
                    case 10:
                        var1 = 1.1186943620178043;
                        var2 = 1.0214285714285714;
                        break;

                    case 11:
                        var1 = 1.0505836575875487;
                        var2 = 0.940809968847352;
                        break;

                    case 12:
                        var1 = 1.0103703703703704;
                        var2 = 0.94390243902439019;
                        break;
                }
                goto font_0AC3;
            }
            if (!(m_font.FontName == "Calibri") || (m_font.Size != 10))
            {
                if ((m_font.FontName == "Calibri") && (m_font.Size == 12))
                {
                    var1 = 1.0;
                    var2 = 1.0238095238095237;
                    goto font_0AC3;
                }
                if (!(m_font.FontName == "Calibri") || (m_font.Size != 11))
                {
                    if (!m_font.FontName.StartsWith("ï¼­ï¼³") || (m_font.Size != 9))
                    {
                        if (!m_font.FontName.StartsWith("ï¼­ï¼³") || (m_font.Size != 10))
                        {
                            if (!m_font.FontName.StartsWith("ï¼­ï¼³") || (m_font.Size != 11))
                            {
                                if (!m_font.FontName.StartsWith("ï¼­ï¼³") || (m_font.Size != 12))
                                {
                                    if (!(m_font.FontName == "Arial") || (m_font.Size != 9))
                                    {
                                        if ((m_font.FontName == "Arial") && (m_font.Size == 10))
                                        {
                                            var1 = 1.0521920668058455;
                                            var2 = 0.96791443850267378;
                                            goto font_0AC3;
                                        }
                                        if (!(m_font.FontName == "Arial") || (m_font.Size != 11))
                                        {
                                            if ((m_font.FontName == "Arial") && (m_font.Size == 8))
                                            {
                                                var1 = 0.98663697104677062;
                                                var2 = 0.90625;
                                                goto font_0AC3;
                                            }
                                            if ((m_font.FontName == "Geneva") && (m_font.Size == 9))
                                            {
                                                var1 = 1.0807600950118765;
                                                var2 = 1.0551470588235294;
                                                goto font_0AC3;
                                            }
                                            if ((m_font.FontName == "Verdana") && (m_font.Size == 10))
                                            {
                                                var1 = 1.0553633217993079;
                                                var2 = 1.013840830449827;
                                                goto font_0AC3;
                                            }
                                            if ((m_font.FontName == "Verdana") && (m_font.Size == 8))
                                            {
                                                var1 = 0.96072013093289688;
                                                var2 = 0.93965517241379315;
                                                goto font_0AC3;
                                            }
                                            if ((m_font.FontName == "Arial MT") && (m_font.Size == 12))
                                            {
                                                var1 = 0.995850622406639;
                                                var2 = 0.983402489626556;
                                                goto font_0AC3;
                                            }
                                            if ((m_font.FontName == "Tahoma") && (m_font.Size == 10))
                                            {
                                                var1 = 1.0260416666666667;
                                                var2 = 1.0;
                                                goto font_0AC3;
                                            }
                                            if ((m_font.FontName == "MS Sans Serif") && (m_font.Size == 8))
                                            {
                                                var1 = 0.90064102564102566;
                                                var2 = 0.95483870967741935;
                                                goto font_0AC3;
                                            }
                                            int num12 = sheet.PageSetup.PrintQuality;
                                            if (num12 <= 100)
                                            {
                                                if (num12 != 0x60)
                                                {
                                                    if (num12 != 100)
                                                    {
                                                        goto font_0A6D;
                                                    }
                                                    var1 = 1.0944350758853287;
                                                    var2 = 0.96074380165289253;
                                                }
                                                else
                                                {
                                                    var1 = 1.0015432098765431;
                                                    var2 = 1.0010764262648009;
                                                }
                                                goto font_0AC3;
                                            }
                                            switch (num12)
                                            {
                                                case 120:
                                                    var1 = 1.0268987341772151;
                                                    var2 = 0.98831030818278431;
                                                    goto font_0AC3;

                                                case 200:
                                                    var1 = 1.0962837837837838;
                                                    var2 = 0.98831030818278431;
                                                    goto font_0AC3;

                                                case 600:
                                                    var1 = 1.0535714285714286;
                                                    var2 = 0.97077244258872653;
                                                    goto font_0AC3;
                                            }
                                            goto font_0A6D;
                                        }
                                        int printQlty1 = sheet.PageSetup.PrintQuality;
                                        if (printQlty1 <= 100)
                                        {
                                            if (printQlty1 != 0x60)
                                            {
                                                if (printQlty1 != 100)
                                                {
                                                    goto font_0831;
                                                }
                                                var1 = 0.9600591715976331;
                                                var2 = 0.90998043052837574;
                                            }
                                            else
                                            {
                                                var1 = 1.0015432098765431;
                                                var2 = 0.94801223241590216;
                                            }
                                            goto font_0AC3;
                                        }
                                        switch (printQlty1)
                                        {
                                            case 120:
                                                var1 = 1.0;
                                                var2 = 0.96875;
                                                goto font_0AC3;

                                            case 200:
                                                var1 = 1.020440251572327;
                                                var2 = 0.96074380165289253;
                                                goto font_0AC3;

                                            case 600:
                                                var1 = 1.020440251572327;
                                                var2 = 0.9617373319544984;
                                                goto font_0AC3;
                                        }
                                        goto font_0831;
                                    }
                                    int prntQlty2 = sheet.PageSetup.PrintQuality;
                                    if (prntQlty2 <= 100)
                                    {
                                        if (prntQlty2 != 0x60)
                                        {
                                            if (prntQlty2 != 100)
                                            {
                                                goto font_070B;
                                            }
                                            var1 = 0.9600591715976331;
                                            var2 = 1.0208562019758507;
                                        }
                                        else
                                        {
                                            var1 = 1.0015432098765431;
                                            var2 = 1.0010764262648009;
                                        }
                                        goto font_0AC3;
                                    }
                                    switch (prntQlty2)
                                    {
                                        case 120:
                                            var1 = 0.91279887482419131;
                                            var2 = 0.90029041626331074;
                                            goto font_0AC3;

                                        case 200:
                                            var1 = 0.9600591715976331;
                                            var2 = 0.93;
                                            goto font_0AC3;

                                        case 600:
                                            var1 = 0.94814814814814818;
                                            var2 = 0.93093093093093093;
                                            goto font_0AC3;
                                    }
                                    goto font_070B;
                                }
                                int printQlty3 = sheet.PageSetup.PrintQuality;
                                if (printQlty3 <= 100)
                                {
                                    if (printQlty3 != 0x60)
                                    {
                                        if (printQlty3 != 100)
                                        {
                                            goto font_061A;
                                        }
                                        var1 = 1.0798668885191347;
                                        var2 = 1.0108695652173914;
                                    }
                                    else
                                    {
                                        var1 = 1.0015432098765431;
                                        var2 = 1.0010764262648009;
                                    }
                                    goto font_0AC3;
                                }
                                switch (printQlty3)
                                {
                                    case 120:
                                        var1 = 1.0;
                                        var2 = 0.96875;
                                        goto font_0AC3;

                                    case 200:
                                        var1 = 1.020440251572327;
                                        var2 = 0.9346733668341709;
                                        goto font_0AC3;

                                    case 600:
                                        var1 = 1.0015432098765431;
                                        var2 = 0.92721834496510469;
                                        goto font_0AC3;
                                }
                                goto font_061A;
                            }
                            int printQlty4 = sheet.PageSetup.PrintQuality;
                            if (printQlty4 <= 100)
                            {
                                if (printQlty4 != 0x60)
                                {
                                    if (printQlty4 != 100)
                                    {
                                        goto font_0529;
                                    }
                                    var1 = 0.9600591715976331;
                                    var2 = 0.96074380165289253;
                                }
                                else
                                {
                                    var1 = 1.0015432098765431;
                                    var2 = 1.0010764262648009;
                                }
                                goto font_0AC3;
                            }
                            switch (printQlty4)
                            {
                                case 120:
                                    var1 = 0.90013869625520115;
                                    var2 = 0.9337349397590361;
                                    goto font_0AC3;

                                case 200:
                                    var1 = 0.9600591715976331;
                                    var2 = 0.9337349397590361;
                                    goto font_0AC3;

                                case 600:
                                    var1 = 0.921875;
                                    var2 = 0.908203125;
                                    goto font_0AC3;
                            }
                            goto font_0529;
                        }
                        int printQlty5 = sheet.PageSetup.PrintQuality;
                        if (printQlty5 <= 100)
                        {
                            if (printQlty5 != 0x60)
                            {
                                if (printQlty5 != 100)
                                {
                                    goto font_0438;
                                }
                                var1 = 0.9600591715976331;
                                var2 = 1.0208562019758507;
                            }
                            else
                            {
                                var1 = 1.0015432098765431;
                                var2 = 1.0010764262648009;
                            }
                            goto font_0AC3;
                        }
                        switch (printQlty5)
                        {
                            case 120:
                                var1 = 1.0268987341772151;
                                var2 = 1.0;
                                goto font_0AC3;

                            case 200:
                                var1 = 0.9600591715976331;
                                var2 = 0.96074380165289253;
                                goto font_0AC3;

                            case 600:
                                var1 = 0.96148148148148149;
                                var2 = 0.93093093093093093;
                                goto font_0AC3;
                        }
                        goto font_0438;
                    }
                    int printQlty6 = sheet.PageSetup.PrintQuality;
                    if (printQlty6 <= 100)
                    {
                        if (printQlty6 != 0x60)
                        {
                            if (printQlty6 != 100)
                            {
                                goto font_0347;
                            }
                            var1 = 1.1151202749140894;
                            var2 = 1.0242290748898679;
                        }
                        else
                        {
                            var1 = 1.0015432098765431;
                            var2 = 1.0010764262648009;
                        }
                        goto font_0AC3;
                    }
                    switch (printQlty6)
                    {
                        case 120:
                            var1 = 1.0709570957095709;
                            var2 = 0.95975232198142413;
                            goto font_0AC3;

                        case 200:
                            var1 = 1.0384;
                            var2 = 0.92814371257485029;
                            goto font_0AC3;

                        case 600:
                            var1 = 1.0156494522691706;
                            var2 = 0.908203125;
                            goto font_0AC3;
                    }
                    goto font_0347;
                }
                int prntQlty7 = sheet.PageSetup.PrintQuality;
                if (prntQlty7 <= 100)
                {
                    if (prntQlty7 != 0x60)
                    {
                        if (prntQlty7 != 100)
                        {
                            goto font_0256;
                        }
                        var1 = 0.9600591715976331;
                        var2 = 0.91265947006869474;
                    }
                    else
                    {
                        var1 = 1.0015432098765431;
                        var2 = 0.950920245398773;
                    }
                    goto font_0AC3;
                }
                switch (prntQlty7)
                {
                    case 120:
                        var1 = 1.0268987341772151;
                        var2 = 0.91988130563798221;
                        goto font_0AC3;

                    case 200:
                        var1 = 1.0962837837837838;
                        var2 = 0.96074380165289253;
                        goto font_0AC3;

                    case 600:
                        var1 = 1.0700389105058366;
                        var2 = 0.96666666666666667;
                        goto font_0AC3;
                }
                goto font_0256;
            }
            int printQlty8 = sheet.PageSetup.PrintQuality;
            if (printQlty8 <= 100)
            {
                if (printQlty8 != 0x60)
                {
                    if (printQlty8 != 100)
                    {
                        goto font_0130;
                    }
                    var1 = 0.9600591715976331;
                    var2 = 1.0163934426229508;
                }
                else
                {
                    var1 = 1.0015432098765431;
                    var2 = 0.94224924012158051;
                }
                goto font_0AC3;
            }
            switch (printQlty8)
            {
                case 120:
                    var1 = 1.0268987341772151;
                    var2 = 1.0356347438752784;
                    goto font_0AC3;

                case 200:
                    var1 = 0.9600591715976331;
                    var2 = 1.0449438202247192;
                    goto font_0AC3;

                case 600:
                    var1 = 0.96148148148148149;
                    var2 = 1.0276243093922652;
                    goto font_0AC3;
            }
        font_0130:
            var1 = 0.96148148148148149;
            var2 = 1.0367892976588629;
            goto font_0AC3;
        font_0256:
            var1 = 1.0518638573743921;
            var2 = 0.96074380165289253;
            goto font_0AC3;
        font_0347:
            var1 = 1.0140625;
            var2 = 0.93939393939393945;
            goto font_0AC3;
        font_0438:
            var1 = 0.96148148148148149;
            var2 = 0.96074380165289253;
            goto font_0AC3;
        font_0529:
            var1 = 0.921875;
            var2 = 0.92537313432835822;
            goto font_0AC3;
        font_061A:
            var1 = 1.0015432098765431;
            var2 = 0.9441624365482234;
            goto font_0AC3;
        font_070B:
            var1 = 0.96148148148148149;
            var2 = 0.96074380165289253;
            goto font_0AC3;
        font_0831:
            var1 = 1.0417335473515248;
            var2 = 0.97791798107255523;
            goto font_0AC3;
        font_0A6D:
            var1 = 1.0518638573743921;
            var2 = 0.97997892518440466;
        font_0AC3: ;
            return new double[] { var1, var2 };
        }
    }
}
