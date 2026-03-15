#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.XlsIO;
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Manager;
using Syncfusion.Windows.Grid.Olap;
using System.Windows.Media;

namespace Syncfusion.Windows.Grid.Olap
{
    /// <summary>
    /// GridWordExport exports the Pivot data to Word with the applied style
    /// </summary>
    public class GridWordExport
    {
        #region Private Members

        private RowFormat format;
        private WTextBody textBody;
        private int col = 0, temp = 0, count = 6;
        private PivotEngine Engine { get; set; }
        private ExportingGridStyleInfo GridExportStyle { get; set; }
        private GridLayout Layout { get; set; }
        private const string m_Total = "Total";
        private const string m_GrandTotal = "Grand Total";

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GridWordExport"/> class.
        /// </summary>
        /// <param name="DrillResult">The drill result.</param>
        /// <param name="AllowTableBreak">if set to <c>true</c> [allow table break].</param>
        /// <param name="gridLayout">The grid layout.</param>
        public GridWordExport(PivotEngine DrillResult, GridLayout gridLayout)
        {
            this.Engine = DrillResult;
            this.Layout = gridLayout;            
        }

        /// <summary>
        /// Exports the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="styleinfo">The styleinfo.</param>
        public void Export(string filename, ExportingGridStyleInfo styleinfo)
        {
            WordDocument document = new WordDocument();
            ///Define Section to render data in declared document
            IWSection section;
            ///To define word paragraphs
            IWParagraph paragraph;
            ///Add sections in document . 
            section = document.AddSection();
            ///Add paragraps in Section
            paragraph = section.AddParagraph();
            ///sets the page size of the word document
            section.PageSetup.PageSize = new SizeF(1200, Engine.RowsCount * 100);

            this.GridExportStyle = styleinfo;
            for (int col = 0; col < Engine[0, 0].Range.Width; col++)
            {
                PivotColumnDescriptor colDesc = Engine.TableColumns[col];
                for (int cell = 0; cell < Engine[0, 0].Range.Height; cell++)
                {
                    PivotCellDescriptor cellDescriptor = colDesc.Cells[cell];
                    cellDescriptor.CellValue = string.Empty;
                    cellDescriptor.CellType = PivotCellDescriptorType.Any;
                }
            }

            for (col = 7; col < Engine.TableColumns.Count; col += 7)
            {
                paragraph = section.AddParagraph();
                ///sets the page size of the word document
                section.PageSetup.PageSize = new SizeF(1200, Engine.RowsCount * 85);
                ///format paragraph
                paragraph.ParagraphFormat.BeforeSpacing = 18f;
                ///Define a section content
                textBody = section.Body;
                ///Create instance for table in word
                IWTable docTable = textBody.AddTable();
                ///set the margin of the setion
                section.PageSetup.Margins.All = 20f;
                ///Formats the table values 
                format = new RowFormat();

                float pageWidth = 0f;
                format.Borders.BorderType = BorderStyle.Single;
                format.Borders.LineWidth = 1.0F;
                format.CellSpacing = 0;

                format.Borders.Color = System.Drawing.Color.Blue;
                docTable.ResetCells(Engine.RowsCount + 1, 7, format, 140);
                for (int row = 0; row <= Engine.RowsCount - 1; row++)
                {
                    #region Format

                    int cellIndex = 0;
                    for (int c = temp; c <= count; c++)
                    {
                        ///Iterate the pivot data from pivot engine and render in to word table
                        string Value = Engine.TableColumns[c].Cells[row].CellValue.ToString();

                        PivotCellDescriptor descriptorvalue = Engine.TableColumns[c].Cells[row];
                        if (descriptorvalue.CellType == PivotCellDescriptorType.Value || descriptorvalue.CellExTypes.Count > 0)
                        {
                            if (descriptorvalue.CellExTypes.Count > 0)
                            {
                                if (descriptorvalue.CellExTypes.Contains(PivotCellDescriptorType.SummaryColumn.ToString()))
                                {
                                    IWTextRange summary = docTable.Rows[row].Cells[cellIndex].AddParagraph().AppendText(Value);
                                    summary.CharacterFormat.TextColor = ColorTranslator.FromHtml(GridExportStyle.SummaryColumnForegroundColor);
                                    summary.CharacterFormat.FontName = styleinfo.SummaryFontName;
                                    summary.CharacterFormat.FontSize = (float)styleinfo.SummaryFontSize;

                                }
                                else if (descriptorvalue.CellExTypes.Contains(PivotCellDescriptorType.SummaryRow.ToString()) && this.Layout != GridLayout.ExcelLikeLayout)
                                {
                                    IWTextRange summary = docTable.Rows[row].Cells[cellIndex].AddParagraph().AppendText(Value);
                                    summary.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.SummaryRowForegroundColor);
                                    summary.CharacterFormat.FontName = styleinfo.SummaryFontName;
                                    summary.CharacterFormat.FontSize = (float)styleinfo.SummaryFontSize;
                                    docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.SummaryRowBackgroundColor);

                                }
                                else
                                {
                                    if (Layout == GridLayout.ExcelLikeLayout && descriptorvalue.CellType != PivotCellDescriptorType.RowHeader && descriptorvalue.CellType != PivotCellDescriptorType.SummaryRow)
                                    {
                                        IWTextRange text = docTable.Rows[row].Cells[cellIndex].AddParagraph().AppendText(Value);
                                        text.CharacterFormat.FontSize = (float)styleinfo.CellFontSize;
                                        text.CharacterFormat.FontName = styleinfo.CellFontName;
                                        text.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.ValueTextColor);
                                        docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.SummaryColumnBackgroundColor);// Color(styleinfo.SummaryColumnBackgroundColor);
                                    }
                                }
                            }
                            else
                            {
                                IWTextRange text = docTable.Rows[row].Cells[cellIndex].AddParagraph().AppendText(Value);
                                text.CharacterFormat.FontSize = (float)styleinfo.CellFontSize;
                                text.CharacterFormat.FontName = styleinfo.CellFontName;
                                text.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.ValueTextColor);
                            }
                        }
                        ///Apply color format

                        PivotCellDescriptor descriptor = Engine.TableColumns[c].Cells[row];
                        float width = docTable.Rows[1].Cells[cellIndex].Width * descriptor.Range.Width;
                        if (descriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                        {
                            docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.HeaderBackgroundColor);
                            IWTextRange text = docTable.Rows[row].Cells[cellIndex].AddParagraph().AppendText(Value);
                            text.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.HeaderForeGroundColor);
                            text.CharacterFormat.FontName = GridExportStyle.HeaderFontName != "" ? GridExportStyle.HeaderFontName : "Times New Roman";
                            text.CharacterFormat.FontSize = (float)GridExportStyle.HeaderFontSize;
                            if (descriptor.Range.Height > 1)
                            {
                                docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                for (int r = row + 1; r < descriptor.Range.Height + row; r++)
                                {
                                    docTable.Rows[r].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Continue;
                                }
                            }
                        }
                        if (descriptor.CellType == PivotCellDescriptorType.RowHeader)
                        {
                            docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.HeaderRowBackgroundColor);
                            IWTextRange text = docTable.Rows[row].Cells[cellIndex].AddParagraph().AppendText(Value);
                            text.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.HeaderRowForegroundColor);
                            text.CharacterFormat.FontName = GridExportStyle.HeaderFontName != "" ? GridExportStyle.HeaderFontName : "Times New Roman";
                            text.CharacterFormat.FontSize = (float)GridExportStyle.HeaderFontSize;
                            if (descriptor.Range.Height > 1)
                            {
                                docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                for (int r = row + 1; r < descriptor.Range.Height + row; r++)
                                {
                                    docTable.Rows[r].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Continue;
                                }
                            }
                            if (descriptor.Range.Width > 1)
                            {
                                docTable.Rows[row].Cells[cellIndex].CellFormat.HorizontalMerge = CellMerge.Start;
                                for (int cell = cellIndex + 1; cell < descriptor.Range.Width + cellIndex; cell++)
                                {
                                    docTable.Rows[row].Cells[cell].CellFormat.HorizontalMerge = CellMerge.Continue;
                                }
                            }

                        }
                        if (descriptor.CellType == PivotCellDescriptorType.SummaryColumn)
                        {
                            docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.SummaryColumnBackgroundColor);
                            IWTextRange text = docTable.Rows[row].Cells[cellIndex].AddParagraph().AppendText(Value);
                            text.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.SummaryColumnForegroundColor);
                            text.CharacterFormat.FontName = GridExportStyle.SummaryFontName != "" ? GridExportStyle.SummaryFontName : "Times New Roman";
                            text.CharacterFormat.FontSize = (float)GridExportStyle.SummaryFontSize;
                            if (descriptor.CellValue == m_Total)
                            {
                                for (int i = row; i < Engine.RowsCount; i++)
                                {
                                    docTable.Rows[i].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.SummaryColumnBackgroundColor);
                                }
                            }
                            if (descriptor.Range.Height > 1)
                            {
                                docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                for (int r = row + 1; r < descriptor.Range.Height + row; r++)
                                {
                                    docTable.Rows[r].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Continue;
                                }
                            }
                        }
                        if (descriptor.CellType == PivotCellDescriptorType.SummaryRow)
                        {
                            docTable.Rows[row].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.SummaryRowBackgroundColor);

                            IWTextRange text = docTable.Rows[row].Cells[cellIndex].AddParagraph().AppendText(Value);
                            text.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.SummaryRowForegroundColor);
                            text.CharacterFormat.FontName = GridExportStyle.SummaryFontName != "" ? GridExportStyle.SummaryFontName : "Times New Roman";
                            text.CharacterFormat.FontSize = (float)GridExportStyle.SummaryFontSize;
                            if (descriptor.CellValue == m_Total || descriptor.CellValue == m_GrandTotal)
                            {
                                if (this.Layout != GridLayout.ExcelLikeLayout)
                                {
                                    for (int i = cellIndex; i < count; i++)
                                    {
                                        docTable.Rows[row].Cells[i].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.SummaryRowBackgroundColor);
                                    }
                                }
                                else
                                {
                                    for (int i = cellIndex; i < count + 1; i++)
                                    {
                                        docTable.Rows[row].Cells[i].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.SummaryRowBackgroundColor);
                                    }
                                }
                            }

                            if (descriptor.Range.Height > 1)
                            {
                                docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                for (int r = row + 1; r < descriptor.Range.Height + row; r++)
                                {
                                    docTable.Rows[r].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Continue;
                                }
                            }
                            if (descriptor.Range.Width > 1)
                            {
                                docTable.Rows[row].Cells[cellIndex].CellFormat.HorizontalMerge = CellMerge.Start;
                                for (int cell = cellIndex + 1; cell < descriptor.Range.Width + cellIndex; cell++)
                                {
                                    docTable.Rows[row].Cells[cell].CellFormat.HorizontalMerge = CellMerge.Continue;
                                }
                            }
                        }
                        docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
                        cellIndex++;
                        pageWidth = pageWidth + width;
                    }
                    #endregion
                }
                count += 7;
                temp = (count - 6);
            }
            if ((col - 7) < Engine.TableColumns.Count)
            {
                ///Define a section content
                paragraph = section.AddParagraph();
                ///sets the page size of the word document
                section.PageSetup.PageSize = new SizeF(1200, Engine.RowsCount * 85);
                ///format paragraph
                paragraph.ParagraphFormat.BeforeSpacing = 18f;
                ///Define a section content
                textBody = section.Body;
                ///Create instance for table in word
                IWTable docTable = textBody.AddTable();
                ///set the margin of the setion
                section.PageSetup.Margins.All = 20f;
                ///Formats the table values 
                format = new RowFormat();

                float pageWidth = 0f;
                format.Borders.BorderType = BorderStyle.Single;
                format.Borders.LineWidth = 1.0F;
                format.CellSpacing = 0;

                format.Borders.Color = System.Drawing.Color.Blue;
                if (Engine.TableColumns.Count - 7 > 0)
                {
                    docTable.ResetCells(Engine.RowsCount + 1, Engine.TableColumns.Count - temp, format, 140);
                }
                else
                {
                    docTable.ResetCells(Engine.RowsCount + 1, Engine.TableColumns.Count, format, 140);
                }

                ///Iterate the pivot remaining data  from pivot engine and render in to word table
                for (int r = 0; r <= Engine.RowsCount - 1; r++)
                {
                    int cellIndex = 0;
                    for (int c = (col - 7); c < Engine.TableColumns.Count; c++)
                    {
                        string Value = Engine.TableColumns[c].Cells[r].CellValue.ToString();
                        ///Apply color format
                        ///
                        PivotCellDescriptor descriptor = Engine.TableColumns[c].Cells[r];
                        if (descriptor.CellType == PivotCellDescriptorType.Value || descriptor.CellExTypes.Count > 0)
                        {
                            if (descriptor.CellExTypes.Count > 0)
                            {
                                if (descriptor.CellExTypes.Contains(PivotCellDescriptorType.SummaryColumn.ToString()))
                                {
                                    IWTextRange summary = docTable.Rows[r].Cells[cellIndex].AddParagraph().AppendText(Value);
                                    summary.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.SummaryColumnForegroundColor);
                                    summary.CharacterFormat.FontName = styleinfo.SummaryFontName;
                                    summary.CharacterFormat.FontSize = (float)styleinfo.SummaryFontSize;

                                }
                                else if (descriptor.CellExTypes.Contains(PivotCellDescriptorType.SummaryRow.ToString()))
                                {
                                    IWTextRange summary = docTable.Rows[r].Cells[cellIndex].AddParagraph().AppendText(Value);
                                    summary.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.SummaryRowForegroundColor);
                                    summary.CharacterFormat.FontName = styleinfo.SummaryFontName;
                                    summary.CharacterFormat.FontSize = (float)styleinfo.SummaryFontSize;
                                    docTable.Rows[r].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.SummaryRowBackgroundColor);
                                }
                                else
                                {
                                    IWTextRange text = docTable.Rows[r].Cells[cellIndex].AddParagraph().AppendText(Value);
                                    text.CharacterFormat.FontSize = (float)styleinfo.CellFontSize;
                                    text.CharacterFormat.FontName = styleinfo.CellFontName;
                                    text.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.ValueTextColor);
                                }
                            }
                            else
                            {
                                IWTextRange text = docTable.Rows[r].Cells[cellIndex].AddParagraph().AppendText(Value);
                                text.CharacterFormat.FontSize = (float)styleinfo.CellFontSize;
                                text.CharacterFormat.FontName = styleinfo.CellFontName;
                                text.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.ValueTextColor);
                            }
                        }
                        if (descriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                        {
                            docTable.Rows[r].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.HeaderBackgroundColor);
                            IWTextRange text = docTable.Rows[r].Cells[cellIndex].AddParagraph().AppendText(Value);
                            text.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.HeaderForeGroundColor);
                            text.CharacterFormat.FontName = GridExportStyle.HeaderFontName != "" ? GridExportStyle.HeaderFontName : "Times New Roman";
                            text.CharacterFormat.FontSize = (float)GridExportStyle.HeaderFontSize;
                            if (descriptor.Range.Height > 1)
                            {
                                docTable.Rows[r].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                for (int row = r + 1; row < descriptor.Range.Height + r; row++)
                                {
                                    docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Continue;
                                }
                            }
                        }
                        if (descriptor.CellType == PivotCellDescriptorType.RowHeader)
                        {
                            docTable.Rows[r].Cells[c].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.HeaderRowBackgroundColor);
                            IWTextRange text = docTable.Rows[r].Cells[cellIndex].AddParagraph().AppendText(Value);
                            text.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.HeaderRowForegroundColor);
                            text.CharacterFormat.FontName = GridExportStyle.HeaderFontName != "" ? GridExportStyle.HeaderFontName : "Times New Roman";
                            text.CharacterFormat.FontSize = (float)GridExportStyle.HeaderFontSize;
                            docTable.Rows[r].Height = 15;
                            if (descriptor.Range.Height > 1)
                            {
                                docTable.Rows[r].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                for (int row = r + 1; row < descriptor.Range.Height + r; row++)
                                {
                                    docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Continue;
                                }
                            }
                            if (descriptor.Range.Width > 1)
                            {
                                docTable.Rows[r].Cells[cellIndex].CellFormat.HorizontalMerge = CellMerge.Start;
                                for (int cell = cellIndex + 1; cell < descriptor.Range.Width + cellIndex; cell++)
                                {
                                    docTable.Rows[r].Cells[cell].CellFormat.HorizontalMerge = CellMerge.Continue;
                                }
                            }
                        }
                        if (descriptor.CellType == PivotCellDescriptorType.SummaryColumn)
                        {
                            docTable.Rows[r].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.SummaryColumnBackgroundColor);

                            IWTextRange text = docTable.Rows[r].Cells[cellIndex].AddParagraph().AppendText(Value);
                            text.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.SummaryColumnForegroundColor);
                            text.CharacterFormat.FontName = GridExportStyle.SummaryFontName != "" ? GridExportStyle.SummaryFontName : "Times New Roman";
                            text.CharacterFormat.FontSize = (float)GridExportStyle.SummaryFontSize;
                            if (descriptor.CellValue == m_Total)
                            {
                                for (int i = r; i < Engine.RowsCount; i++)
                                {
                                    docTable.Rows[i].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.SummaryColumnBackgroundColor);
                                }
                            }

                            if (descriptor.Range.Height > 1)
                            {
                                docTable.Rows[r].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                for (int row = r + 1; row < descriptor.Range.Height + r; row++)
                                {
                                    docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Continue;
                                }
                            }
                        }
                        if (descriptor.CellType == PivotCellDescriptorType.SummaryRow)
                        {
                            docTable.Rows[r].Cells[cellIndex].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.SummaryRowBackgroundColor);

                            IWTextRange text = docTable.Rows[r].Cells[cellIndex].AddParagraph().AppendText(Value);
                            text.CharacterFormat.TextColor = ColorTranslator.FromHtml(styleinfo.SummaryRowForegroundColor);
                            text.CharacterFormat.FontName = GridExportStyle.SummaryFontName != "" ? GridExportStyle.SummaryFontName : "Times New Roman";
                            text.CharacterFormat.FontSize = (float)GridExportStyle.SummaryFontSize;
                            if (descriptor.CellValue == m_Total)
                            {
                                for (int i = cellIndex; i < Engine.TableColumns.Count; i++)
                                {
                                    docTable.Rows[r].Cells[i].CellFormat.BackColor = ColorTranslator.FromHtml(styleinfo.SummaryRowBackgroundColor);
                                }
                            }

                            if (descriptor.Range.Height > 1)
                            {
                                docTable.Rows[r].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Start;
                                for (int row = r + 1; row < descriptor.Range.Height + r; row++)
                                {
                                    docTable.Rows[row].Cells[cellIndex].CellFormat.VerticalMerge = CellMerge.Continue;
                                }
                            }
                            if (descriptor.Range.Width > 1)
                            {
                                docTable.Rows[r].Cells[cellIndex].CellFormat.HorizontalMerge = CellMerge.Start;
                                for (int cell = cellIndex + 1; cell < descriptor.Range.Width + cellIndex; cell++)
                                {
                                    docTable.Rows[r].Cells[cell].CellFormat.HorizontalMerge = CellMerge.Continue;
                                }
                            }
                        }
                        docTable.Rows[r].Cells[cellIndex].CellFormat.VerticalAlignment = Syncfusion.DocIO.DLS.VerticalAlignment.Middle;
                        cellIndex++;
                    }
                }
                ///Save the iterated document to opened document
                document.Save(filename, FormatType.Doc);
            }
        }     
    }
}
       

