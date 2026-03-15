#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Styles;
using Syncfusion.XlsIO;
using Syncfusion.Windows.ComponentModel;
using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.Windows.Controls.Spreadsheet.CommandExtensions;
using System.IO;
using System.Text.RegularExpressions;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Controls.Spreadsheet.Commands;
using System.Collections;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.Spreadsheet
{

    /// <summary>
    /// Holds undo information about a previous <see cref="SpreadsheetGridModel.ChangeCells"/> operation.
    /// </summary>
    /// <remarks>
    /// The <see cref="Execute"/> method will call <see cref="SpreadsheetGridModel.ChangeCells"/> 
    /// with information stored in this object and scroll the affect range into the current grid view.
    /// </remarks>
    public class SpreadsheetGridCommand : GridChangeCellsCommand 
    {
        public IWorksheet worksheet;
        public GridRangeInfo gridRange;
        public GridStyleInfo[] gridCellsInfo;
        SpreadsheetGridModel spreadsheetGridModel;
        public StyleInfoProperty changedSip;

        /// <summary>
        /// Initializes the <see cref="GridChangeCellsCommand"/> with information how to execute
        /// a <see cref="SpreadsheetGridModel.ChangeCells"/> command at a later time.
        /// </summary>
        /// <param name="table">The <see cref="SpreadsheetGridModel"/> this command is associated with.</param>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellsInfo">The array of <see cref="GridStyleInfo"/> objects that holds cell information.</param>
        /// <param name="type">A <see cref="StyleModifyType"/> that specifies the style operation to be performed.</param>
        /// <param name="sip">A <see cref="StyleInfoProperty"/> that specifies the changed property.</param>
        public SpreadsheetGridCommand(SpreadsheetGridModel table, GridRangeInfo range, GridStyleInfo[] cellsInfo, StyleModifyType type, StyleInfoProperty sip)
            :base(table, range, cellsInfo, type)
        {
            worksheet = table.ExcelProperties.WorkBook.Worksheets[table.SheetName];
            gridRange = range;
            gridCellsInfo = cellsInfo;
            spreadsheetGridModel = table;
            changedSip = sip;
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        public override void Execute()
        { 
            base.Execute();

            int cellIndex;
            for (int rowIndex = gridRange.Top; rowIndex <= gridRange.Bottom; rowIndex++)
            {
                for (int colIndex = gridRange.Left; colIndex <= gridRange.Right; colIndex++)
                {
                    cellIndex = ((rowIndex - gridRange.Top) * gridRange.Width) + (colIndex - gridRange.Left);
                    if (colIndex == 0 || rowIndex == 0)
                        continue;
                    IRange rangeToConvert = worksheet[rowIndex, colIndex];
                    GridStyleInfo styleInfo = gridCellsInfo[cellIndex];
                    ExportCellToExcel(rangeToConvert, styleInfo);

                    if (spreadsheetGridModel.SperadsheetGrid.CurrentCell.CellRowColumnIndex == new RowColumnIndex(rowIndex, colIndex))
                        spreadsheetGridModel.ExcelProperties.spreadControl.GridProperties.RefreshCurrentStyle();
                }
            }
            spreadsheetGridModel.ActiveGridView.InvalidateCells();
        }

        /// <summary>
        /// Export grid style info to excel range <see cref="IRange"/>.
        /// </summary>
        /// <param name="rangeToConvert">A <see cref="IRange"/> that specifies the excel range.</param>
        /// <param name="styleInfo">A <see cref="GridStyleInfo"/> that specifies the range of cells.</param>
        public void ExportCellToExcel(IRange rangeToConvert, GridStyleInfo styleInfo)
        {
            SetFontToExcelRange(rangeToConvert, styleInfo);
            SetAlignment(rangeToConvert, styleInfo);
            SetStyle(rangeToConvert, styleInfo);
            SetBorder(rangeToConvert, styleInfo);
            SetBrush(rangeToConvert, styleInfo);            
            SetConditionalFormat(rangeToConvert, styleInfo);
            SetHyperLink(rangeToConvert, styleInfo);
            GridCommitCellInfoEventArgs arg = new GridCommitCellInfoEventArgs(styleInfo.CellRowColumnIndex, styleInfo, changedSip);
            ImportExportHelper.ExportCellToExcel(spreadsheetGridModel, spreadsheetGridModel.ExcelProperties.WorkBook, rangeToConvert, arg);
        }

        /// <summary>
        /// Export fond info to excel range <see cref="IRange"/>.
        /// </summary>
        /// <param name="rangeToConvert">A <see cref="IRange"/> that specifies the excel range.</param>
        /// <param name="styleInfo">A <see cref="GridStyleInfo"/> that specifies the range of cells.</param>
        private void SetFontToExcelRange(IRange rangeToConvert, GridStyleInfo styleInfo)
        {
            IStyle iStyle = rangeToConvert.CellStyle;
            iStyle.Font.FontName = styleInfo.Font.FontFamily.Source;
            if (styleInfo.Font.TextDecorations == TextDecorations.Underline)
                iStyle.Font.Underline = ExcelUnderline.Single;
            else
                iStyle.Font.Underline = ExcelUnderline.None;

            if (styleInfo.Font.FontWeight == FontWeights.Bold)
                iStyle.Font.Bold = true;
            else
                iStyle.Font.Bold = false;

            if (styleInfo.Font.FontStyle == FontStyles.Italic)
                iStyle.Font.Italic = true;
            else
                iStyle.Font.Italic = false;            
        }

        /// <summary>
        /// Export alignment style to excel range <see cref="IRange"/>.
        /// </summary>
        /// <param name="rangeToConvert">A <see cref="IRange"/> that specifies the excel range.</param>
        /// <param name="styleInfo">A <see cref="GridStyleInfo"/> that specifies the range of cells.</param>
        private void SetAlignment(IRange rangeToConvert, GridStyleInfo styleInfo)
        {
            if (styleInfo.HorizontalAlignment == HorizontalAlignment.Center)
                rangeToConvert.HorizontalAlignment = ExcelHAlign.HAlignCenter;
            else if (styleInfo.HorizontalAlignment == HorizontalAlignment.Right)
                rangeToConvert.HorizontalAlignment = ExcelHAlign.HAlignRight;
            else 
                rangeToConvert.HorizontalAlignment = ExcelHAlign.HAlignLeft;

            if (styleInfo.VerticalAlignment ==  VerticalAlignment.Center)
                rangeToConvert.VerticalAlignment = ExcelVAlign.VAlignCenter;
            else if (styleInfo.VerticalAlignment == VerticalAlignment.Top)
                rangeToConvert.VerticalAlignment = ExcelVAlign.VAlignTop;
            else
                rangeToConvert.VerticalAlignment = ExcelVAlign.VAlignBottom;
        }

        /// <summary>
        /// Set excel border style to <see cref="IRange"/> from <see cref="GridStyleInfo"/>.
        /// </summary>
        /// <param name="rangeToConvert">A <see cref="IRange"/> that specifies the excel range.</param>
        /// <param name="styleInfo">A <see cref="GridStyleInfo"/> that specifies the range of cells.</param>
        private void SetExcelBorderStyle(IRange rangeToConvert, GridStyleInfo styleInfo)
        {
            double bottomThickness = styleInfo.Borders.Bottom != null ? styleInfo.Borders.Bottom.Thickness : 0;
            double leftThickness = styleInfo.Borders.Left != null ? styleInfo.Borders.Left.Thickness : 0;
            double rightThickness = styleInfo.Borders.Right != null ? styleInfo.Borders.Right.Thickness : 0;
            double topThickness = styleInfo.Borders.Top != null ? styleInfo.Borders.Top.Thickness : 0;

#if SILVERLIGHT
            if (styleInfo.Borders.Bottom != null && styleInfo.Borders.Bottom.Style != BorderStyle.Standard)
                bottomThickness = 0.0;
            if (styleInfo.Borders.Left != null && styleInfo.Borders.Left.Style != BorderStyle.Standard)
                leftThickness = 0.0;
            if (styleInfo.Borders.Right != null && styleInfo.Borders.Right.Style != BorderStyle.Standard)
                rightThickness = 0.0;
            if (styleInfo.Borders.Top != null && styleInfo.Borders.Top.Style != BorderStyle.Standard)
                topThickness = 0.0;
#endif
            
            CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.NoBorder);

            if (bottomThickness == 0.50 && leftThickness == 0.50 &&
                rightThickness == 0.50 && topThickness == 0.50)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.AllBorder);

            if (bottomThickness == 0 && leftThickness == 0 &&
               rightThickness == 0 && topThickness == 0)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.NoBorder);

            if (bottomThickness == 2.00 && leftThickness == 2.00 &&
                rightThickness == 2.00 && topThickness == 2.00)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.ThickBoxBorder);

            if (bottomThickness == 0.50 && topThickness == 0.50)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.TopAndBottomBorder);

            if (bottomThickness == 2.00 && topThickness == 2.00)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.TopAndThickBottomBorder);

            if (bottomThickness == 0.50)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.BottomBorder);
            else if (bottomThickness == 2.00)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.ThickBottomBorder);
            

            if (leftThickness == 0.50)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.LeftBorder);
            else if (leftThickness == 2.00)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.ThickLeftBorder);
            

            if (rightThickness == 0.50)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.RightBorder);
            else if (rightThickness == 2.00)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.ThickRightBorder);
            

            if (topThickness == 0.50)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.TopBorder);
            else if (topThickness == 2.00)
                CommandExtensions.CommandExtensions.ChangeCellBorder(rangeToConvert, ExcelBorderStyle.ThickTopBorder);
            
        }

        /// <summary>
        /// Method to set border to excel range <see cref="IRange"/>.
        /// </summary>
        /// <param name="rangeToConvert">A <see cref="IRange"/> that specifies the excel range.</param>
        /// <param name="styleInfo">A <see cref="GridStyleInfo"/> that specifies the range of cells.</param>
        private void SetBorder(IRange rangeToConvert, GridStyleInfo styleInfo)
        {
            if (styleInfo.HasBorders)
            {
                SetExcelBorderStyle(rangeToConvert, styleInfo);
                this.Grid.InvalidateVisual();
            }
        }

        /// <summary>
        /// Method to set cell style to excel range <see cref="IRange"/>.
        /// </summary>
        /// <param name="rangeToConvert">A <see cref="IRange"/> that specifies the excel range.</param>
        /// <param name="styleInfo">A <see cref="GridStyleInfo"/> that specifies the range of cells.</param>
        private void SetStyle(IRange rangeToConvert, GridStyleInfo styleInfo)
        {
            if (styleInfo.TextWrapping == TextWrapping.Wrap)
            {
                rangeToConvert.WrapText = true;                
            }
            else
            {
                rangeToConvert.WrapText = false;
            }

            rangeToConvert.IndentLevel = (int)styleInfo.TextMargins.Left / 10;
            
            if (string.IsNullOrEmpty(styleInfo.Format))
                rangeToConvert.NumberFormat = "General";
            else
                rangeToConvert.NumberFormat = styleInfo.Format;

            if (styleInfo.Comment != null)
            {                                
                rangeToConvert.AddComment();
                rangeToConvert.Comment.Text = styleInfo.Comment;
            }
            else
            {
                if (rangeToConvert.Comment!=null)
                    rangeToConvert.Comment.Remove();
            }

            this.Grid.InvalidateVisual();       
        }

        /// <summary>
        /// Method to set the foreground and background to excel range <see cref="IRange"/>.
        /// </summary>
        /// <param name="rangeToConvert">A <see cref="IRange"/> that specifies the excel range.</param>
        /// <param name="styleInfo">A <see cref="GridStyleInfo"/> that specifies the range of cells.</param>
        private void SetBrush(IRange rangeToConvert, GridStyleInfo styleInfo)
        {
#if SILVERLIGHT
            var backColor = (styleInfo.Background as SolidColorBrush).Color;
            var foreColor = (styleInfo.Foreground as SolidColorBrush).Color;
            
            if (rangeToConvert.CellStyle.Color != backColor)
            {
                if (backColor != Color.FromArgb(255,255,255,255))
                    rangeToConvert.CellStyle.Color = backColor;
                else
                    rangeToConvert.CellStyle.FillPattern = ExcelPattern.None;
            }
            if (rangeToConvert.CellStyle.Font.RGBColor != foreColor)
                rangeToConvert.CellStyle.Font.RGBColor = foreColor;  
            
#else
            var styleColor = (styleInfo.Background as SolidColorBrush).Color;
            var textColor = (styleInfo.Foreground as SolidColorBrush).Color;
            System.Drawing.Color foreColor = System.Drawing.Color.FromArgb(textColor.A, textColor.R, textColor.G, textColor.B);
            System.Drawing.Color backColor = System.Drawing.Color.FromArgb(styleColor.A, styleColor.R, styleColor.G, styleColor.B);

            if (rangeToConvert.CellStyle.Color != backColor)
            {
                if (styleColor != Brushes.White.Color)
                    rangeToConvert.CellStyle.Color = backColor;
                else
                    rangeToConvert.CellStyle.FillPattern = ExcelPattern.None;
            }
            if(rangeToConvert.CellStyle.Font.RGBColor !=foreColor)
                rangeToConvert.CellStyle.Font.RGBColor = foreColor;                      
            
            this.Grid.InvalidateCell(GridRangeInfo.Cell(styleInfo.RowIndex, styleInfo.ColumnIndex));
            this.Grid.InvalidateVisual();
#endif
        }

        /// <summary>
        /// Method to set the Hyperlink to the Excel Range.
        /// </summary>
        /// <param name="rangeToConvert">Excel range</param>
        /// <param name="styleInfo">GridStyleInfo</param>
        private void SetHyperLink(IRange rangeToConvert, GridStyleInfo styleInfo)
        {
            string pattern = @"((https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";
            
            if (styleInfo.CellType == "HyperLink")
            {
                IHyperLink hyperLink = rangeToConvert.Hyperlinks.Add(rangeToConvert);

                if (Regex.IsMatch(styleInfo.CellValue2.ToString(), pattern))
                    hyperLink.Type = ExcelHyperLinkType.Url;
                else
                    hyperLink.Type = ExcelHyperLinkType.Workbook;
                
                hyperLink.TextToDisplay = styleInfo.CellValue.ToString();
                hyperLink.Address = styleInfo.CellValue2.ToString();                
            }
            else
            {
                if (rangeToConvert.Hyperlinks.Count > 0)
                {
                    rangeToConvert.Worksheet.HyperLinks.RemoveAt(rangeToConvert.Count - 1);                                                       
                }
            }

            ExcelGridModelImportExtensions.CopyHyperlinkToCell(rangeToConvert.Worksheet.Workbook, rangeToConvert.Worksheet, rangeToConvert, styleInfo);
            this.spreadsheetGridModel.InvalidateCell(rangeToConvert.ConvertExcelRangeToGridRange());            
        }

        /// <summary>
        /// Method to set the Conditional Format to the Excel Range.
        /// </summary>
        /// <param name="rangeToConvert">Specifies the Excel range</param>
        /// <param name="styleInfo">StyleInfo from Grid Cell</param>
        private void SetConditionalFormat(IRange rangeToConvert, GridStyleInfo styleInfo)
        {
            if (styleInfo.HasConditionalFormat)
            {
                IConditionalFormats conditions = rangeToConvert.ConditionalFormats;
                IConditionalFormat condition1 = conditions.AddCondition();

                condition1.FormatType = ExcelCFType.CellValue;

                if (styleInfo.ConditionalFormat.Conditions.Count == 2)
                {
                    condition1.Operator = ExcelComparisonOperator.Between;
                    condition1.FirstFormula = styleInfo.ConditionalFormat.Conditions[0].Value.ToString();
                    condition1.SecondFormula = styleInfo.ConditionalFormat.Conditions[1].Value.ToString();
                }
                else
                {
                    condition1.Operator = GetExcelOperator(styleInfo.ConditionalFormat.Conditions[0].ConditionType.ToString());
                    condition1.FirstFormula = styleInfo.ConditionalFormat.Conditions[0].Value.ToString();
                }

                Color backColor = ((SolidColorBrush)styleInfo.ConditionalFormat.Style.Background).Color;
                Color foreColor = ((SolidColorBrush)styleInfo.ConditionalFormat.Style.Foreground).Color;
#if SILVERLIGHT
                condition1.BackColorRGB = backColor;
                condition1.FontColorRGB = foreColor;
#else
                condition1.BackColorRGB = System.Drawing.Color.FromArgb(backColor.A, backColor.R, backColor.G, backColor.B);                              
                condition1.FontColorRGB = System.Drawing.Color.FromArgb(foreColor.A, foreColor.R, foreColor.G, foreColor.B);
#endif



                if (styleInfo.ConditionalFormat.Style.Borders.HasBottom && styleInfo.ConditionalFormat.Style.Borders.HasTop &&
                    styleInfo.ConditionalFormat.Style.Borders.HasLeft && styleInfo.ConditionalFormat.Style.Borders.HasRight)
                {
                    Color bottomColor = ((SolidColorBrush)styleInfo.ConditionalFormat.Style.Borders.Bottom.Brush).Color;
#if SILVERLIGHT
                    condition1.BottomBorderColorRGB = bottomColor;
                    condition1.TopBorderColorRGB = bottomColor;
                    condition1.LeftBorderColorRGB = bottomColor;
                    condition1.RightBorderColorRGB = bottomColor;
#else
                    condition1.BottomBorderColorRGB = System.Drawing.Color.FromArgb(bottomColor.A, bottomColor.R, bottomColor.G, bottomColor.B);
                    condition1.TopBorderColorRGB = System.Drawing.Color.FromArgb(bottomColor.A, bottomColor.R, bottomColor.G, bottomColor.B);
                    condition1.LeftBorderColorRGB = System.Drawing.Color.FromArgb(bottomColor.A, bottomColor.R, bottomColor.G, bottomColor.B);
                    condition1.RightBorderColorRGB = System.Drawing.Color.FromArgb(bottomColor.A, bottomColor.R, bottomColor.G, bottomColor.B);
#endif
                }                
            }
            else
            {
                for (int i = 0; i < rangeToConvert.ConditionalFormats.Count; i++)
                {
                    rangeToConvert.ConditionalFormats.Remove();                    
                }
            }
        }


        /// <summary>
        /// Parse the Enum StyleInfo.ConditionalFormats.ConditionType.
        /// </summary>
        /// <param name="conditionType">StyleInfo.ConditionalFormats.ConditionType as String</param>
        /// <returns>ExcelComparisonOperator</returns>
       private ExcelComparisonOperator GetExcelOperator(string conditionType)
        {            
            switch (conditionType)
            {
                case "LessThan":
                    return ExcelComparisonOperator.Less;                    

                case "GreaterThan":
                    return ExcelComparisonOperator.Greater;                   

                case "Equals":
                    return ExcelComparisonOperator.Equal;
                    
                case "NotEquals":
                    return ExcelComparisonOperator.NotEqual;                    

                case "LessThanOrEqual":
                    return ExcelComparisonOperator.LessOrEqual;                    

                case "GreaterThanOrEqual":
                    return ExcelComparisonOperator.GreaterOrEqual;                    

                default:
                    return ExcelComparisonOperator.None;
            }
        }

    }

    /// <summary>
    /// Holds undo information about <see cref="CommandExtensions.CommandExtensions.ChangeFontSize"/> method.
    /// </summary>
    /// <remarks>
    /// This method Push SpreadsheetFontSizeChangedCommand into the CommandStack and 
    /// Changes the FontStyle stored in this object to the Excel sheet.
    ///</remarks>
    public class SpreadsheetFontSizeChangedCommand : GridChangeCellsCommand
    {
        IWorksheet workSheet;
        SpreadsheetGridModel spreadsheetGridModel;
        GridRangeInfo gridRange;
        GridStyleInfo[] gridCellsInfo;

        public SpreadsheetFontSizeChangedCommand(SpreadsheetGridModel table, GridRangeInfo range, GridStyleInfo[] cellsInfo, StyleModifyType type)
            : base(table, range, cellsInfo, type)
        {
            workSheet = table.ExcelProperties.WorkBook.ActiveSheet;
            spreadsheetGridModel = table;
            gridRange = range;
            gridCellsInfo = cellsInfo;
        }

        public override void Execute()
        {
            this.spreadsheetGridModel.CommandStack.BeginTrans("Fontsize changed");
            GridStyleInfo[] savedCellsInfo = this.spreadsheetGridModel.GetCellsInfo(gridRange);
            SpreadsheetFontSizeChangedCommand fontCommand = new SpreadsheetFontSizeChangedCommand(this.spreadsheetGridModel, gridRange, savedCellsInfo, StyleModifyType.Copy);
            this.spreadsheetGridModel.CommandStack.Push(fontCommand);

            int cellIndex;
            for (int rowIndex = gridRange.Top; rowIndex <= gridRange.Bottom; rowIndex++)
            {
                for (int colIndex = gridRange.Left; colIndex <= gridRange.Right; colIndex++)
                {
                    cellIndex = ((rowIndex - gridRange.Top) * gridRange.Width) + (colIndex - gridRange.Left);
                    IRange rangeToConvert = workSheet[rowIndex, colIndex];
                    GridStyleInfo styleInfo = gridCellsInfo[cellIndex];
                    GridRangeInfo gridResizeRange =GridRangeInfo.Cell(rowIndex, colIndex);
                    double fontSize=styleInfo.Font.FontSize / 1.2;
                    spreadsheetGridModel[rowIndex, colIndex].Font.FontSize = styleInfo.Font.FontSize;
                    if (spreadsheetGridModel.SperadsheetGrid.CurrentCell.CellRowColumnIndex == new RowColumnIndex(rowIndex, colIndex))
                    {
                        spreadsheetGridModel.ExcelProperties.spreadControl.GridProperties.RefreshCurrentStyle();
                    }
                    if (rangeToConvert.CellStyle.Font.Size != fontSize)
                    {
                        rangeToConvert.CellStyle.Font.Size = fontSize;
                        this.spreadsheetGridModel.CommandStack.SuspendUndo = true;
                        this.spreadsheetGridModel.ResizeRowsToFit(gridResizeRange, GridResizeToFitOptions.None);

                        //When we reduce the font size then the row height also reduced below the default value.
                        for (int row = gridResizeRange.Top; row <= gridResizeRange.Bottom; row++)
                        {
                            if (this.spreadsheetGridModel.RowHeights[row] < 24)
                                this.spreadsheetGridModel.RowHeights[row] = 24;
                        }

                        this.spreadsheetGridModel.CommandStack.SuspendUndo = false;
                    }
                }
            }
            this.spreadsheetGridModel.CommandStack.CommitTrans();
            this.spreadsheetGridModel.InvalidateCell(gridRange);                                   
        }
    }

    /// <summary>
    /// This command object holds row size information to undo row height of excel range.
    /// </summary>
    public class SpreadsheetSetRowSizeCommand : GridModelSetRowSizeCommand
    {
        IWorksheet worksheet;
        private int From;
        private int Count;
        private double Values;

        /// <summary>
        /// Initializes a new SpreadsheetSetRowSizeCommand object with all the commands.
        /// </summary>
        /// <param name="model">The <see cref="SpreadsheetGridModel"/> this command is associated with.</param>
        /// <param name="from">First row index in range.</param>
        /// <param name="last">Last row index in range.</param>
        /// <param name="values">The sizes to be applies to the range.</param>
        public SpreadsheetSetRowSizeCommand(SpreadsheetGridModel model, int from, int last, double values)
            :base(model, from, last, values)
        {
            worksheet = model.ExcelProperties.WorkBook.ActiveSheet;
            From = from;
            Count = (last + 1) - from;
            Values = values;
        }

        /// <summary>
        /// Exceute the command.
        /// </summary>
        public override void Execute()
        {
            base.Execute();
            worksheet.SetRowHeightInPixels(From, Values);
        }

    }

    /// <summary>
    /// This command object holds column size information to undo column width of excel range.
    /// </summary>
    public class SpreadsheetSetColumnSizeCommand : GridModelSetColumnSizeCommand
    {
        IWorksheet worksheet;
        int Count;
        int From;
        double Values;

        /// <summary>
        /// Initializes a new SpreadsheetSetColumnSizeCommand object with all the commands.
        /// </summary>
        /// <param name="model">The <see cref="SpreadsheetGridModel"/> this command is associated with.</param>
        /// <param name="from">First row index in range.</param>
        /// <param name="last">Last row index in range.</param>
        /// <param name="values">The sizes to be applies to the range.</param>
        public SpreadsheetSetColumnSizeCommand(SpreadsheetGridModel model, int from, int last, double values)
            :base(model, from, last, values)
        {
            worksheet = model.ExcelProperties.WorkBook.ActiveSheet;
            Count = (last + 1) - from;
            From = from;
            Values = values;
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public override void Execute()
        {
            base.Execute();            
            worksheet.SetColumnWidthInPixels(From, Count, (int)Values);
        }
    }

    public class SpreadsheetSetRowHideCommand : GridModelSetRowHideCommand
    {
        IWorksheet worksheet;
        private int from;
        private int to;
        bool shouldHide;
        SpreadsheetGridModel spreadsheetGridModel;

        public SpreadsheetSetRowHideCommand(SpreadsheetGridModel model, int first, int last, bool hide)
            : base(model, first, last, hide)
        {
            spreadsheetGridModel = model;
            this.from = first;
            this.to = last;
            shouldHide = hide;
            worksheet = model.ExcelProperties.WorkBook.ActiveSheet;            
        }

        public override void Execute()
        {
            if (shouldHide)
            {
                SpreadsheetSetRowHideCommand hideCommand = new SpreadsheetSetRowHideCommand(spreadsheetGridModel, from, to, false);
                spreadsheetGridModel.CommandStack.Push(hideCommand);
            }
            else
            {
                SpreadsheetSetRowHideCommand hideCommand = new SpreadsheetSetRowHideCommand(spreadsheetGridModel, from, to, true);
                spreadsheetGridModel.CommandStack.Push(hideCommand);
            }
            spreadsheetGridModel.CommandStack.SuspendUndo = true;
            spreadsheetGridModel.RowHeights.SetHidden(from, to, shouldHide);
            spreadsheetGridModel.CommandStack.SuspendUndo = false;
            spreadsheetGridModel.InvalidateCell(GridRangeInfo.Rows(from, to));
        }
    }

    public class SpreadsheetSetColumnHideCommand : GridModelSetColumnHideCommand
    {
        IWorksheet worksheet;
        private int from;
        private int to;
        bool shouldHide;
        SpreadsheetGridModel spreadsheetGridModel;

        public SpreadsheetSetColumnHideCommand(SpreadsheetGridModel model, int first, int last, bool hide)
            : base(model, first, last, hide)
        {
            spreadsheetGridModel = model;
            this.from = first;
            this.to = last;
            shouldHide = hide;
            worksheet = model.ExcelProperties.WorkBook.ActiveSheet;            
        }


        public override void Execute()
        {
            if (shouldHide)
            {
                SpreadsheetSetColumnHideCommand hideCommand = new SpreadsheetSetColumnHideCommand(spreadsheetGridModel, from, to, false);
                spreadsheetGridModel.CommandStack.Push(hideCommand);
            }
            else
            {
                SpreadsheetSetColumnHideCommand hideCommand = new SpreadsheetSetColumnHideCommand(spreadsheetGridModel, from, to, true);
                spreadsheetGridModel.CommandStack.Push(hideCommand);
            }
            spreadsheetGridModel.CommandStack.SuspendUndo = true;
            spreadsheetGridModel.ColumnWidths.SetHidden(from, to, shouldHide);
            spreadsheetGridModel.CommandStack.SuspendUndo = false;
            spreadsheetGridModel.InvalidateCell(GridRangeInfo.Cols(from, to));
        }

    }
    /// <summary>
    /// Holds undo information about a previous "GridModelRowColOperations.InsertRange" operation.
    /// </summary>
    public class SpreadsheetModelRemoveRowsCommand : GridModelRemoveRowsCommand
    {
        private int from;
        private int rowCount;
        private SpreadsheetGridModel spreadsheetGridModel;
        
        /// <summary>
        /// Initializes the <see cref="SpreadsheetModelRemoveRowsCommand"/> with information how to execute
        /// a "GridModelRowColOperations.RemoveRange" command at a later time and associates it with a "GridModelRowColOperations"
        /// instance.
        /// </summary>
        /// <param name="model">The <see cref="SpreadsheetGridModel"/> this command is associated with.</param>
        /// <param name="from">First row index in range.</param>
        /// <param name="count">Last row index in range.</param>
        public SpreadsheetModelRemoveRowsCommand(SpreadsheetGridModel model, int from, int count)
            : base(model, from, count)
        {
            //SetDescription(SR.GetString("CommandRemoveRows", from, last));
            spreadsheetGridModel = model;
            this.from = from;
            this.rowCount = count;
        }

        /// <override/>
        /// <summary>Executes the command.</summary>
        public override void Execute()
        {
            double[] rowHeights = new double[rowCount];
            int arrayIndex = 0;
            for (int rowIndex = this.from; rowIndex <= from + rowCount - 1; rowIndex++)
            {
                rowHeights[arrayIndex] = spreadsheetGridModel.RowHeights[rowIndex];
            }

            GridStyleInfo[] cellsInfo = spreadsheetGridModel.GetCellsInfo(GridRangeInfo.Cells(from, 1, from+rowCount-1, spreadsheetGridModel.ColumnCount - 1));
            GridFormulaEngine formulaEngine = new GridFormulaEngine(spreadsheetGridModel);
            GridFormulaInfo[] formulaInfos = new GridFormulaInfo[1000];
            int formulaIndex = 0;
            foreach (string s in formulaEngine.DependentCells.Keys)
            {
                Hashtable ht = (Hashtable)formulaEngine.DependentCells[s];

                foreach (object o in ht.Keys)
                {
                    string str = o as string;
                    int rowIndex = formulaEngine.RowIndex(str);
                    int colIndex = formulaEngine.ColIndex(str);

                    GridStyleInfo style = new GridStyleInfo();
                    spreadsheetGridModel.IsInInsert = true;
                    spreadsheetGridModel.GetCellInfo(rowIndex, colIndex, style);
                    spreadsheetGridModel.IsInInsert = false;
                    GridFormulaInfo formula = new GridFormulaInfo();
                    formula.RowIndex = rowIndex;
                    formula.ColIndex = colIndex;
                    formula.StyleInfo = style;
                    formulaInfos[formulaIndex] = formula;
                    formulaIndex++;
                }
            }
            spreadsheetGridModel.CommandStack.Push(new SpreadsheetModelInsertRowsCommand(spreadsheetGridModel, from, rowCount, cellsInfo, formulaInfos,rowHeights));

            spreadsheetGridModel.IsInInsert = true;
            Grid.RemoveRows(from, rowCount);
            spreadsheetGridModel.IsInInsert = false;
            spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet.DeleteRow(from, rowCount);
            spreadsheetGridModel.InvalidateCell(GridRangeInfo.Table());
        }
    }

    /// <summary>
    /// Holds undo information about a previous "GridModelRowColOperations.InsertRange" operation.
    /// </summary>
    public class SpreadsheetModelRemoveColumnsCommand : GridModelRemoveColumnsCommand
    {
        private int from;
        private int columnCount;
        private SpreadsheetGridModel spreadsheetGridModel;

        /// <summary>
        /// Initializes the <see cref="SpreadsheetModelRemoveColumnsCommand"/> with information how to execute
        /// a "GridModelRowColOperations.RemoveRange" command at a later time and associates it with a "GridModelRowColOperations"/>
        /// instance.
        /// </summary>
        /// <param name="model">The <see cref="SpreadsheetGridModel"/> this command is associated with.</param>
        /// <param name="from">First row index in range.</param>
        /// <param name="count">Last row index in range.</param>
        public SpreadsheetModelRemoveColumnsCommand(SpreadsheetGridModel model, int from, int count)
            : base(model, from, count)
        {
            // SetDescription(SR.GetString("CommandRemoveColumns", from, last));
            this.from = from;
            this.columnCount = count;
            spreadsheetGridModel = model;
        }

        /// <override/>
        /// <summary>Executes the command.</summary>
        public override void Execute()
        {
            double[] columnWidths = new double[columnCount];
            int arrayIndex = 0;
            for (int columnIndex = this.from; columnIndex <= from + columnCount - 1; columnIndex++)
            {
                columnWidths[arrayIndex] = spreadsheetGridModel.ColumnWidths[columnIndex];
            }
            GridStyleInfo[] cellsInfo = spreadsheetGridModel.GetCellsInfo(GridRangeInfo.Cells(1, from, spreadsheetGridModel.RowCount - 1, from+columnCount-1));
            GridFormulaEngine formulaEngine = new GridFormulaEngine(spreadsheetGridModel);
            GridFormulaInfo[] formulaInfos = new GridFormulaInfo[1000];
            int formulaIndex = 0;
            foreach (string s in formulaEngine.DependentCells.Keys)
            {
                Hashtable ht = (Hashtable)formulaEngine.DependentCells[s];

                foreach (object o in ht.Keys)
                {
                    string str = o as string;
                    int rowIndex = formulaEngine.RowIndex(str);
                    int colIndex = formulaEngine.ColIndex(str);

                    GridStyleInfo style = new GridStyleInfo();
                    spreadsheetGridModel.GetCellInfo(rowIndex, colIndex, style);
                    GridFormulaInfo formula = new GridFormulaInfo();
                    formula.RowIndex = rowIndex;
                    formula.ColIndex = colIndex;
                    formula.StyleInfo = style;
                    formulaInfos[formulaIndex] = formula;
                    formulaIndex++;
                }
            }
            spreadsheetGridModel.CommandStack.Push(new SpreadsheetModelInsertColumnsCommand(spreadsheetGridModel, from, columnCount, cellsInfo,formulaInfos,columnWidths));

            Grid.RemoveColumns(from, columnCount);
            spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet.DeleteColumn(from, columnCount);
            spreadsheetGridModel.InvalidateCell(GridRangeInfo.Table());
        }
    }

    /// <summary>
    /// Holds undo information about a previous "GridModelRowColOperations.InsertRange" operation.
    /// </summary>
    public class SpreadsheetModelInsertRowsCommand : GridModelInsertRowsCommand
    {
        private int insertAt;
        private int count;
        private SpreadsheetGridModel spreadsheetGridModel;
        GridStyleInfo[] gridCellsInfo;
        IWorksheet workSheet;
        StyleInfoProperty changedSip = GridStyleInfoStore.CellValueProperty;
        GridFormulaInfo[] formulaInfo;
        double[] rowHeight;
        
        /// <summary>
        /// Initializes the <see cref="SpreadsheetModelInsertRowsCommand"/> with information how to execute
        /// a "GridModelRowColOperations.RemoveRange" command at a later time and associates it with a "GridModelRowColOperations"/>
        /// instance.
        /// </summary>
        /// <param name="model">The <see cref="SpreadsheetGridModel"/> this command is associated with.</param>
        /// <param name="insertAt">The row or column where cells should be inserted.</param>
        /// <param name="count">The number of rows or columns to insert.</param>
        /// <param name="cellsInfo">The collection of GridStyleInfo.</param>
        /// <param name="formulas">The Collection of GridFormulaInfo.</param>
        /// <param name="heights">The RowHeights.</param>
        public SpreadsheetModelInsertRowsCommand(SpreadsheetGridModel model, int insertAt, int count, GridStyleInfo[] cellsInfo, GridFormulaInfo[] formulas,double[] heights)
            : base(model, insertAt, count)
        {
            // SetDescription(SR.GetString("CommandInsertRows", count, insertAt));
            workSheet = model.ExcelProperties.WorkBook.ActiveSheet;
            this.insertAt = insertAt;
            this.count = count;
            spreadsheetGridModel = model; 
            gridCellsInfo = cellsInfo;
            formulaInfo = formulas;
            rowHeight = heights;
        }
        
        /// <override/>
        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            spreadsheetGridModel.IsInInsert = true;
            spreadsheetGridModel.InsertRows(insertAt, count);
            spreadsheetGridModel.IsInInsert = false;

            spreadsheetGridModel.CommandStack.SuspendUndo = true;
            for (int i = 0; i < count; i++)
            {
                spreadsheetGridModel.RowHeights[insertAt + i] = rowHeight[i];
            }
            spreadsheetGridModel.CommandStack.SuspendUndo = false;

            spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet.InsertRow(insertAt, count);

            for (int i = 0; i < count; i++)
            {
                spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet.SetRowHeightInPixels(insertAt + i, (int)rowHeight[i]);
            }

            SpreadsheetGridCommand cmd = new SpreadsheetGridCommand(spreadsheetGridModel, GridRangeInfo.Empty, gridCellsInfo, StyleModifyType.Copy, GridStyleInfoStore.CellValueProperty);
            int cellIndex=0;
            for (int rowIndex = insertAt; rowIndex <= insertAt+count-1; rowIndex++)
            {
                for (int colIndex = 1; colIndex <= spreadsheetGridModel.ColumnCount-1; colIndex++)
                {                    
                    IRange rangeToConvert = workSheet[rowIndex, colIndex];
                    GridStyleInfo styleInfo = gridCellsInfo[cellIndex];
                    cmd.ExportCellToExcel(rangeToConvert, styleInfo);
                    cellIndex++;                                        
                }
            }
            spreadsheetGridModel.ActiveGridView.InvalidateCells();

            
            foreach (var o in formulaInfo)
            {
                if (o != null)
                {
                    IRange rngtoConvert = workSheet[(o as GridFormulaInfo).RowIndex, (o as GridFormulaInfo).ColIndex];
                    cmd.ExportCellToExcel(rngtoConvert, (o as GridFormulaInfo).StyleInfo);                    
                    spreadsheetGridModel.InvalidateCell(GridRangeInfo.Cell((o as GridFormulaInfo).RowIndex, (o as GridFormulaInfo).ColIndex));
                }
            }
                        
        }
             
    }

    /// <summary>
    /// Holds undo information about a previous "GridModelRowColOperations.InsertRange" operation.
    /// </summary>
    public class SpreadsheetModelInsertColumnsCommand : GridModelInsertColumnsCommand
    {
        IWorksheet workSheet;
        private int insertAt;
        private int count;
        private SpreadsheetGridModel gridModel;
        GridStyleInfo[] gridCellsInfo;
        GridFormulaInfo[] formulaInfo;
        double[] columnWidth;

        /// <summary>
        /// Initializes the <see cref="SpreadsheetModelInsertColumnsCommand"/> with information how to execute
        /// a "GridModelRowColOperations.RemoveRange" command at a later time and associates it with a "GridModelRowColOperations"
        /// instance.
        /// </summary>
        /// <param name="model">The <see cref="SpreadsheetGridModel"/> this command is associated with.</param>
        /// <param name="insertAt">The row or column where cells should be inserted.</param>
        /// <param name="count">The number of rows or columns to insert.</param>
        /// <param name="cellsInfo">The collection of GridStyleInfo.</param>
        /// <param name="formulas">The Collection of GridFormulaInfo.</param>
        /// <param name="width">The ColumnWidths.</param>
        public SpreadsheetModelInsertColumnsCommand(SpreadsheetGridModel model, int insertAt, int count,GridStyleInfo[] cellsInfo,GridFormulaInfo[] formulas,double[] width)
            : base(model, insertAt, count)
        {
            this.insertAt = insertAt;
            this.count = count;
            gridModel = model;
            gridCellsInfo = cellsInfo;
            workSheet = model.ExcelProperties.WorkBook.ActiveSheet;
            formulaInfo = formulas;
            columnWidth = width;
        }

        /// <override/>
        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            Grid.InsertColumns(insertAt, count);
            gridModel.CommandStack.SuspendUndo = true;
            for (int i = 0; i < count; i++)
            {
                gridModel.ColumnWidths[insertAt+i] = columnWidth[i];
            }
            gridModel.CommandStack.SuspendUndo = false;
                
            gridModel.ExcelProperties.WorkBook.ActiveSheet.InsertColumn(insertAt, count);

            for (int i = 0; i < count; i++)
            {                
                gridModel.ExcelProperties.WorkBook.ActiveSheet.SetColumnWidthInPixels(insertAt+i,(int)columnWidth[i]);                
            }

            SpreadsheetGridCommand gridCommand = new SpreadsheetGridCommand(gridModel, GridRangeInfo.Empty, gridCellsInfo, StyleModifyType.Copy, GridStyleInfoStore.CellValueProperty);
            int cellIndex=0;
            
            for (int rowIndex = 1; rowIndex <= gridModel.RowCount - 1; rowIndex++)                
            {
                for (int colIndex = insertAt; colIndex <= insertAt + count - 1; colIndex++)
                {                    
                    IRange rangeToConvert = workSheet[rowIndex, colIndex];

                    GridStyleInfo styleInfo = gridCellsInfo[cellIndex];
                    gridCommand.ExportCellToExcel(rangeToConvert, styleInfo);
                    cellIndex++;                    
                }
            }

            gridModel.InvalidateCell(GridRangeInfo.Table());

            foreach (var o in formulaInfo)
            {
                if (o != null)
                {
                    IRange rngtoConvert = workSheet[(o as GridFormulaInfo).RowIndex, (o as GridFormulaInfo).ColIndex];
                    gridCommand.ExportCellToExcel(rngtoConvert, (o as GridFormulaInfo).StyleInfo);
                    gridModel.InvalidateCell(GridRangeInfo.Cell((o as GridFormulaInfo).RowIndex, (o as GridFormulaInfo).ColIndex));
                }
            }
        }
    }

    /// <summary>
    /// Holds undo information about a previous <see cref="IListObject"/>.
    /// </summary>
    public class SpreadsheetTableFormatCommand : GridChangeCellsCommand
    {
        public IWorksheet worksheet;
        public GridRangeInfo gridRange;
        public GridStyleInfo[] gridCellsInfo;
        SpreadsheetGridModel spreadsheetGridModel;
        IListObject listObject;

        /// <summary>
        /// Initializes the <see cref="SpreadsheetTableFormatCommand"/> with information about table format.
        /// </summary>
        /// <param name="table">The <see cref="SpreadsheetGridModel"/> this command is associated with.</param>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellsInfo">The array of <see cref="GridStyleInfo"/> objects that holds cell information.</param>
        /// <param name="type">A <see cref="StyleModifyType"/> that specifies the style operation to be performed.</param>
        /// <param name="tableFormat">A <see cref="IListObject"/> that specifies the table format to be performed.</param>
        public SpreadsheetTableFormatCommand(SpreadsheetGridModel table, GridRangeInfo range, GridStyleInfo[] cellsInfo, StyleModifyType type, IListObject tableFormat)
            :base(table, range, cellsInfo, type)
        {
            worksheet = table.ExcelProperties.WorkBook.ActiveSheet;
            gridRange = range;
            gridCellsInfo = cellsInfo;
            spreadsheetGridModel = table;
            listObject = tableFormat;
        }

        public override void Execute()
        {
            base.Execute();

            if (worksheet.ListObjects.Contains(listObject))
                worksheet.ListObjects.Remove(listObject);
            else
                worksheet.ListObjects.Add(listObject);            
            
            this.Grid.ActiveGridView.InvalidateCells();
        }
    }

    public class SpreadsheetWraptextCommand : GridChangeCellsCommand
    {
        IWorksheet workSheet;
        SpreadsheetGridModel spreadsheetGridModel;
        GridRangeInfo gridRange;
        GridStyleInfo[] gridCellsInfo;
        bool needToWrap;

        public SpreadsheetWraptextCommand(SpreadsheetGridModel table, GridRangeInfo range, GridStyleInfo[] cellsInfo, StyleModifyType type, bool wrap)
            : base(table, range, cellsInfo, type)
        {
            workSheet = table.ExcelProperties.WorkBook.ActiveSheet;
            spreadsheetGridModel = table;
            gridRange = range;
            gridCellsInfo = cellsInfo;
            needToWrap = wrap;
        }
        public override void Execute()
        {
            SpreadsheetWraptextCommand wrapCommand = new SpreadsheetWraptextCommand(spreadsheetGridModel, gridRange, gridCellsInfo, StyleModifyType.Copy, !needToWrap);
            spreadsheetGridModel.CommandStack.Push(wrapCommand);

            spreadsheetGridModel.CommandStack.SuspendUndo = true;
            for (int rowIndex = gridRange.Top; rowIndex <= gridRange.Bottom; rowIndex++)
            {
                for (int colIndex = gridRange.Left; colIndex <= gridRange.Right; colIndex++)
                {
                    IRange rangeToConvert = workSheet[rowIndex, colIndex];
                    if (needToWrap)
                    {
                        rangeToConvert.WrapText = true;
                        var style = spreadsheetGridModel[rowIndex, colIndex];
#if !SILVERLIGHT
                        style.TextTrimming = TextTrimming.None;
                        style.FloatCellMode = GridFloatCellsMode.None;                        
#else
                            style.FloatCellsMode = GridFloatCellsMode.None;
                            style.EnableFloatCell = false;
#endif
                        style.TextWrapping = TextWrapping.Wrap;
                        spreadsheetGridModel.Options.WrapCell = true;
                            
                    }
                    else
                    {
                        rangeToConvert.WrapText = false;
                        var style = spreadsheetGridModel[rowIndex, colIndex];
                        style.TextWrapping = TextWrapping.NoWrap;
                        
#if !SILVERLIGHT
                        style.FloatCellMode = GridFloatCellsMode.OnDemandCalculation;
                        style.TextTrimming = TextTrimming.CharacterEllipsis;
#else
                        style.TextTrimming = TextTrimming.None;
                        style.FloatCellsMode = GridFloatCellsMode.OnDemandCalculation;
#endif                         
                    }
                }
            }

            spreadsheetGridModel.ResizeRowsToFit(gridRange, GridResizeToFitOptions.None);

            for (int i = gridRange.Top; i <= gridRange.Bottom; i++)                   
                    spreadsheetGridModel.RowHeights[i] = workSheet.GetRowHeightInPixels(i);            

            spreadsheetGridModel.CommandStack.SuspendUndo = false;
            spreadsheetGridModel.InvalidateCell(gridRange);
            //spreadsheetGridModel.ActiveGridView.InvalidateCells();

                
        }
    }
    public class SpreadsheetGridLinesCommand:GridChangeCellsCommand
    {
        IWorksheet workSheet;
        SpreadsheetGridModel spreadsheetGridModel;
        GridStyleInfo[] styleInfo = new GridStyleInfo[] { };
        bool showGridLines = false;
        public SpreadsheetGridLinesCommand(SpreadsheetGridModel table,GridRangeInfo range,GridStyleInfo[]cellsInfo,StyleModifyType type,bool showLines):base(table, range, cellsInfo, type)
        {
            workSheet = table.ExcelProperties.WorkBook.ActiveSheet;
            spreadsheetGridModel = table;
            showGridLines = showLines;
        }

        public override void Execute()
        {
            this.Grid.ActiveGridView.Model.CommandStack.Push(new SpreadsheetGridLinesCommand(spreadsheetGridModel, GridRangeInfo.Empty, styleInfo, StyleModifyType.Copy,spreadsheetGridModel.ActiveGridView.ShowGridLines));
            workSheet.IsGridLinesVisible = showGridLines;
            this.Grid.ActiveGridView.ShowGridLines = showGridLines;
            this.Grid.ActiveGridView.Model.InvalidateVisual();            
        }
    }

    /// <summary>
    /// Holds undo information about a previous merge cell range.
    /// </summary>
    public class SpreadsheetMergeCommand : GridChangeCellsCommand
    {
        public IWorksheet worksheet;
        public GridRangeInfo gridRange;
        public GridStyleInfo[] gridCellsInfo;
        SpreadsheetGridModel spreadsheetGridModel;
        string mergeRange;
        bool canMerge;

        /// <summary>
        /// Initializes the <see cref="SpreadsheetTableFormatCommand"/> with information about merge cell range.
        /// </summary>
        /// <param name="table">The <see cref="SpreadsheetGridModel"/> this command is associated with.</param>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellsInfo">The array of <see cref="GridStyleInfo"/> objects that holds cell information.</param>
        /// <param name="type">A <see cref="StyleModifyType"/> that specifies the style operation to be performed.</param>
        /// <param name="merge">Specify te range to merge or unmerge</param>
        public SpreadsheetMergeCommand(SpreadsheetGridModel table, GridRangeInfo range, GridStyleInfo[] cellsInfo, StyleModifyType type, bool merge)
            : base(table, range, cellsInfo, type)
        {
            worksheet = table.ExcelProperties.WorkBook.ActiveSheet;
            gridRange = range;
            gridCellsInfo = cellsInfo;
            spreadsheetGridModel = table;
            canMerge = merge;
            mergeRange = range.ConvertGridRangeToExcelRange();
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        public override void Execute()
        {                        
            //canMerge = !this.spreadsheetGridModel.CoveredCells.Ranges.Contains(GridRangeInfo.Cells(gridRange.Top, gridRange.Left, gridRange.Bottom, gridRange.Right)) && 
                //worksheet.Range[mergeRange].IsMerged;

            if (spreadsheetGridModel.CommandStack.ShouldGenerateUndoInfo)
            {
                gridCellsInfo = spreadsheetGridModel.GetCellsInfo(gridRange);
                SpreadsheetMergeCommand mergeCommand = new SpreadsheetMergeCommand(spreadsheetGridModel, gridRange, gridCellsInfo, StyleModifyType.Copy, !canMerge);
                spreadsheetGridModel.CommandStack.Push(mergeCommand);
            }

            if (canMerge)
            {
                worksheet.Range[mergeRange].Merge();
                worksheet.Range[mergeRange].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range[mergeRange].VerticalAlignment = ExcelVAlign.VAlignCenter;
            }
            else
            {
                worksheet.Range[mergeRange].UnMerge();
                worksheet.Range[mergeRange].HorizontalAlignment = ExcelHAlign.HAlignGeneral;
                worksheet.Range[mergeRange].VerticalAlignment = ExcelVAlign.VAlignBottom;               
            }

            spreadsheetGridModel.CommandStack.SuspendUndo = true;
            spreadsheetGridModel.CoveredCells.Clear();
            ExcelGridModelImportExtensions.CopyMergesToGrid(worksheet, spreadsheetGridModel);
            spreadsheetGridModel.CommandStack.SuspendUndo = false;

            spreadsheetGridModel.InvalidateCell(gridRange);
            spreadsheetGridModel.InvalidateVisual(true);
        }
    }

    /// <summary>
    /// Holds Undo information about previous<see cref="Commands.InsertPictureCommand"/> Operation.
    /// </summary>
    public class SpreadsheetPictureCommand:GridChangeCellsCommand
    {
        public IWorksheet workSheet;
        public GridRangeInfo gridRange;
        SpreadsheetGridModel gridModel;
        MemoryStream imgStream;
        string excelRange;
        bool AddPicture;
        GridStyleInfo[] cellsInfo = new GridStyleInfo[] { };
        public SpreadsheetPictureCommand(SpreadsheetGridModel table, GridRangeInfo range,GridStyleInfo[] cellsInfo, StyleModifyType type, MemoryStream stream,bool addpic)
            :base(table,range,cellsInfo,type)
        {
            workSheet = table.ExcelProperties.WorkBook.ActiveSheet;
            gridRange = range;
            imgStream = stream;
            gridModel = table;
            excelRange = gridRange.ConvertGridRangeToExcelRange();
            AddPicture = !addpic;
        }

        public override void Execute()
        {
            if (AddPicture)
            {
                workSheet.Pictures.AddPicture(gridRange.Top, gridRange.Left, imgStream);
            }
            else
            {
                if (workSheet.Pictures.Count > 0)
                    workSheet.Pictures[workSheet.Pictures.Count - 1].Remove();
            }

            SpreadsheetPictureCommand pictureCommand = new SpreadsheetPictureCommand(gridModel, gridRange, cellsInfo, StyleModifyType.Copy, imgStream, AddPicture);
            this.Grid.ActiveGridView.Model.CommandStack.Push(pictureCommand);
                        
            if (this.gridModel.GraphicModel.GraphicCells.Count > 0)
            {                
                this.gridModel.GraphicModel.GraphicCells.RemoveAt(this.gridModel.GraphicModel.GraphicCells.Count-1);
               ScrollControlChildFrame canvas= this.gridModel.ActiveGridView.GetChildFrame(false, false, false, false, this.gridModel.ActiveGridView.GraphicFrame);
               if (canvas.Children.Count > 0)
                   canvas.Children.RemoveAt(canvas.Children.Count - 1);
            }
            
            this.gridModel.ActiveGridView.InvalidateCells();
            this.gridModel.CommandStack.SuspendUndo = true;
            ExcelGridModelImportExtensions.CopyImageToGrid(workSheet, this.gridModel);
            this.gridModel.CommandStack.SuspendUndo = false;
            this.gridModel.InvalidateVisual();
        }
    }

    public class SpreadsheetGroupUngroupCommand :GridModelCommand
    {
        SpreadsheetGridModel spreadsheetGridModel;
        IRange excelRange;
        ExcelGroupBy excelGroupBy;
        bool doGroup;

        public SpreadsheetGroupUngroupCommand(SpreadsheetGridModel gridModel, IRange range, ExcelGroupBy groupBy, bool group)
            : base(gridModel)
        {
            this.spreadsheetGridModel = gridModel;
            this.excelRange = range;
            this.excelGroupBy = groupBy;
            this.doGroup = group;
        }

        public override void Execute()
        {            
            if (excelGroupBy == ExcelGroupBy.ByRows)
            {
                if (doGroup)                
                    excelRange.Group(ExcelGroupBy.ByRows, false);                                    
                else
                    excelRange.Ungroup(ExcelGroupBy.ByRows);
                
                this.spreadsheetGridModel.ExcelProperties.spreadControl.OutlineRowCount = (this.spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).RowsOutlineLevel;                

                SpreadsheetGroupUngroupCommand groupCommand = new SpreadsheetGroupUngroupCommand(this.spreadsheetGridModel, excelRange, ExcelGroupBy.ByRows, !doGroup);
                this.spreadsheetGridModel.CommandStack.Push(groupCommand);
            }
            else
            {
                if (doGroup)
                    excelRange.Group(ExcelGroupBy.ByColumns, false);
                else
                    excelRange.Ungroup(ExcelGroupBy.ByColumns);
                
                this.spreadsheetGridModel.ExcelProperties.spreadControl.OutlineColumnCount = (this.spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).ColumnsOutlineLevel;                

                SpreadsheetGroupUngroupCommand groupCommand = new SpreadsheetGroupUngroupCommand(this.spreadsheetGridModel, excelRange, ExcelGroupBy.ByColumns, !doGroup);
                this.spreadsheetGridModel.CommandStack.Push(groupCommand);
            }
        }
    }

    public class SpreadsheetExpandCollapseCommand : GridModelCommand
    {
        SpreadsheetGridModel spreadsheetGridModel;
        IRange excelRange;
        ExcelGroupBy excelGroupBy;
        bool IsExpanded;

        public SpreadsheetExpandCollapseCommand(SpreadsheetGridModel gridModel, IRange range, ExcelGroupBy groupBy, bool isExpanded)
            : base(gridModel)
        {
            this.spreadsheetGridModel = gridModel;
            this.excelRange = range;
            this.excelGroupBy = groupBy;
            this.IsExpanded = isExpanded;
        }

        public override void Execute()
        {
            this.spreadsheetGridModel.CommandStack.Push(new SpreadsheetExpandCollapseCommand(this.spreadsheetGridModel,excelRange,excelGroupBy,!IsExpanded));

            this.spreadsheetGridModel.CommandStack.SuspendUndo = true;
            this.spreadsheetGridModel.IsInGroup = true;
            GridRangeInfo gridRange = excelRange.ConvertExcelRangeToGridRange();
            if (IsExpanded)
            {
                if (this.excelGroupBy == ExcelGroupBy.ByRows)
                {
                    excelRange.CollapseGroup(ExcelGroupBy.ByRows);
                    this.spreadsheetGridModel.RowHeights.SetHidden(gridRange.Top, gridRange.Bottom, true);
                }
                else
                {
                    excelRange.CollapseGroup(ExcelGroupBy.ByColumns);
                    this.spreadsheetGridModel.ColumnWidths.SetHidden(gridRange.Left, gridRange.Right, true);
                }
            }
            else
            {
                if (this.excelGroupBy == ExcelGroupBy.ByRows)
                {
                    excelRange.ExpandGroup(ExcelGroupBy.ByRows);
                    for (int i = gridRange.Top; i <= gridRange.Bottom; i++)
                    {
                        if (this.spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet.Range[i, 1, i, 1].RowHeight > 0)
                            this.spreadsheetGridModel.RowHeights.SetHidden(i, i, false);
                    }
                }
                else
                {
                    excelRange.ExpandGroup(ExcelGroupBy.ByColumns);
                    for (int i = gridRange.Left; i <= gridRange.Right; i++)
                    {
                        if (this.spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet.Range[1, i, 1, i].ColumnWidth > 0)
                            this.spreadsheetGridModel.ColumnWidths.SetHidden(i, i, false);
                    }
                }
            }
            this.spreadsheetGridModel.IsInGroup = false;
            this.spreadsheetGridModel.CommandStack.SuspendUndo = false;
        }
    }
}
