#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.DocIO;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.DocIO.DLS;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Tools.Controls
{
    public class DocxExporting
    {
        #region Constructor

        public DocxExporting()
        {

        }

        #endregion

        #region Private Members
        static ListStyle currentNumListStyle = null;
        static IWSection m_wSection = null;
        static WordDocument word_document = null;
        static IWTextRange textRange = null;
        static IWPicture picture = null;
        #endregion

        #region Methods
        /// <summary>
        /// Converts the DocumentAdv instance to word document stream.
        /// </summary>
        /// <param name="documentadv">The documentadv.</param>
        /// <param name="documentStream">The document stream.</param>
        /// <param name="documentExtension">The word document extension (".doc" or ".docx").</param>
        public static void ConvertToDocument(DocumentAdv documentadv, Stream documentStream, string documentExtension)
        {
            word_document = new WordDocument();
            try
            {
                if (documentExtension == ".docx")
                {
                    SaveAsDocx(GetWordDocument(documentadv), documentStream);
                }
                else if (documentExtension == ".doc")
                {
                    SaveAsDoc(GetWordDocument(documentadv), documentStream);
                }
            }
            catch(Exception)
            {
                new InvalidOperationException("UnSupported Document");
            }
        }


        internal static WordDocument GetWordDocument(DocumentAdv documentadv)
        {   
            m_wSection = word_document.AddSection();
            foreach (SectionAdv section in documentadv.Sections)
            {
                MarginsF margins = new MarginsF(
                    float.Parse((section.PageContentMargin.Left * 0.75).ToString())
                    ,float.Parse((section.PageContentMargin.Top * 0.75).ToString())
                    ,float.Parse((section.PageContentMargin.Right * 0.75).ToString())
                    ,float.Parse((section.PageContentMargin.Bottom * 0.75).ToString()));
                m_wSection.PageSetup.Margins = margins;
                m_wSection.PageSetup.PageSize = new System.Drawing.SizeF(
                    float.Parse((section.PageSize.Width * 0.75).ToString())
                    ,float.Parse((section.PageSize.Height * 0.75).ToString()));
                foreach (BlockAdv block in section.Blocks)
                {
                    if (block is ParagraphAdv)
                    {
                        IWParagraph tempParagraph = m_wSection.AddParagraph();
                        CreateParagraph(block,tempParagraph);
                    }
                    else if (block is TableAdv)
                    {
                        IWTable table=m_wSection.AddTable();
                        CreateTable(block,table);
                    }
                }
                currentNumListStyle = null;
            }
            return word_document;
        }
        internal static IWParagraph CreateParagraph(BlockAdv b,IWParagraph tempParagraph)
        {
            switch ((b as ParagraphAdv).ListType)
            {
                case ListType.None:
                    currentNumListStyle = null;
                    break;
                case ListType.Bulleted:
                    currentNumListStyle = null;
                    tempParagraph.ListFormat.ApplyDefBulletStyle();
                    break;
                case ListType.Numbered:
                    if (currentNumListStyle == null)
                        currentNumListStyle = tempParagraph.Document.AddListStyle(DocIO.DLS.ListType.Numbered, "Numbered_" + Guid.NewGuid().ToString());
                    tempParagraph.ListFormat.ApplyStyle(currentNumListStyle.Name);
                    break;
                default:
                    break;
            }
            foreach (Inline inline in b.Inlines)
            {
                if (inline is SpanAdv)
                {
                    SpanAdv spanning = inline as SpanAdv;
                    textRange = tempParagraph.AppendText(spanning.Text);
                    textRange = ModifySpanAdvCharcterformat(spanning, textRange as WTextRange);
                }
                else if (inline is HyperlinkAdv)
                {
                    HyperlinkAdv hyperLinkData = inline as HyperlinkAdv;
                    string hyperLinkText = hyperLinkData.Text;
                    if (hyperLinkData.NavigationUrl != null)
                        textRange = tempParagraph.AppendHyperlink(hyperLinkData.NavigationUrl, hyperLinkText, HyperlinkType.WebLink);
                    else
                        textRange = tempParagraph.AppendHyperlink("", hyperLinkText, HyperlinkType.Bookmark);
                }
                else if (inline is ImageContainerAdv)
                {
                    ImageContainerAdv imagecontainer = inline as ImageContainerAdv;
                    if (imagecontainer.ImageBytes != null)
                    {
                        picture = tempParagraph.AppendPicture(imagecontainer.ImageBytes);
                        picture.Width = (float)(imagecontainer.Width * 0.75);
                        picture.Height = (float)(imagecontainer.Height * 0.75);
                    }
                }
                else if (inline is UIContainerAdv)
                {
                    // UIContainerAdv uicontainer = inline as UIContainerAdv;

                }
                tempParagraph = GetParagraphformatIndentSpacing(b as ParagraphAdv, tempParagraph as WParagraph);
            }

            return tempParagraph;
        }

        internal static IWTable CreateTable(BlockAdv block,IWTable tempTable)
        {
            TableAdv table=block as TableAdv;
            tempTable.ResetCells(table.Rows.Count, table.TableHolder.Columns.Count);
            foreach (TableRowAdv row in (block as TableAdv).Rows)
            {
                foreach (TableCellAdv cell in row.Cells)
                {
                    foreach (WTableRow wRow in tempTable.Rows)
                    {
                        foreach (WTableCell wCell in wRow.Cells)
                        {
                            if (wCell.GetCellIndex() == cell.ColumnIndex && wRow.GetRowIndex()==cell.RowIndex)
                            {
                                wCell.CellFormat.BackColor = Convert(cell.Background);
                                wCell.CellFormat.Borders.Color = Convert(table.BorderBrush);
                                if (table.BorderThickness == 0.0)
                                {
                                    wCell.CellFormat.Borders.LineWidth = 0.0f;
                                }
                                currentNumListStyle = null;
                                foreach (BlockAdv b in cell.Blocks)
                                {
                                    if (b is ParagraphAdv)
                                    {
                                        IWParagraph para = wCell.AddParagraph();
                                        CreateParagraph(b, para);
                                    }
                                    else if (b is TableAdv)
                                    {
                                        IWTable tabl = wCell.AddTable();
                                        CreateTable(b, tabl);
                                    }
                                }
                            }
                        }
                    }

                    foreach (WTableRow wRow2 in tempTable.Rows)
                    {
                        foreach (WTableCell wCell2 in wRow2.Cells)
                        {
                            if (wCell2.GetCellIndex() == cell.ColumnIndex && wRow2.GetRowIndex() == cell.RowIndex)
                            {
                                if (cell.ColumnSpan > 1 || cell.RowSpan >1)
                                {
                                    wCell2.CellFormat.HorizontalMerge = CellMerge.Start;
                                    int colindex = cell.ColumnSpan;

                                    if (cell.RowSpan > 1)
                                    {
                                        wCell2.CellFormat.VerticalMerge = CellMerge.Start;
                                        int rowindex = cell.RowSpan;

                                        for (int i = 1; i < rowindex; i++)
                                        {
                                            WTableCell wCell3 = GetCellByIndex(tempTable, wCell2.GetCellIndex(), wRow2.GetRowIndex() + i);
                                            wCell3.CellFormat.HorizontalMerge = CellMerge.Start;

                                            for (int j = 1; j < colindex; j++)
                                            {
                                                WTableCell wCell4 = GetCellByIndex(tempTable, wCell3.GetCellIndex() + j, wCell3.OwnerRow.GetRowIndex());
                                                wCell4.CellFormat.HorizontalMerge = CellMerge.Continue;
                                            }
                                            wCell3.CellFormat.VerticalMerge = CellMerge.Continue;
                                        }
                                    }

                                    for (int j = 1; j < colindex; j++)
                                    {
                                        WTableCell wCell4 = GetCellByIndex(tempTable, wCell2.GetCellIndex() + j, wRow2.GetRowIndex());
                                        wCell4.CellFormat.HorizontalMerge = CellMerge.Continue;
                                    }
                                }
                                else if (cell.RowSpan > 1)
                                {
                                    wCell2.CellFormat.VerticalMerge = CellMerge.Start;
                                    int rowindex = cell.RowSpan;

                                    for (int i = 1; i < rowindex; i++)
                                    {
                                        WTableCell wCell3 = GetCellByIndex(tempTable, wCell2.GetCellIndex(), wRow2.GetRowIndex() + i);
                                        wCell3.CellFormat.VerticalMerge = CellMerge.Continue;
                                    }
                                }
                            }
                        }
                    }

                }
            }
            tempTable.TableFormat.IsAutoResized = true;
            return tempTable;
        }


        private static WTableCell GetCellByIndex(IWTable table,int colindex,int rowindex)
        {
            foreach (WTableRow wrw in table.Rows)
            {
                foreach (WTableCell wcl in wrw.Cells)
                {
                    if (wcl.GetCellIndex() == colindex && wrw.GetRowIndex() == rowindex)
                    {
                        return wcl;
                    }
                }
            }
            return null;
        }

        internal static WTextRange ModifySpanAdvCharcterformat(SpanAdv spanning, WTextRange textRange)
        {
            FontStyle fontStyle = spanning.FontStyle;
            if (fontStyle.ToString() == "Italic")
                textRange.CharacterFormat.Italic = true;
            //Text Stirke
            if (spanning.StrikeThrough == StrikeThrough.SingleStrike)
                textRange.CharacterFormat.Strikeout = true;
            else if (spanning.StrikeThrough == StrikeThrough.DoubleStrike)
                textRange.CharacterFormat.DoubleStrike = true;

            //Text Baselining
            textRange.CharacterFormat.SubSuperScript = GetSubSuperScript(spanning.Baseline);

            //Text Foreground and Background Color 
            if (spanning.HighlightColor != null)
            {
                textRange.CharacterFormat.HighlightColor = ConvertHighlightColor(spanning.HighlightColor);
            }

            if (spanning.Foreground != null)
            {
                System.Drawing.Color textColor = Convert(spanning.Foreground);
                textRange.CharacterFormat.TextColor = textColor;
            }

            if (spanning.Underline == true)
                textRange.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;

            textRange.CharacterFormat.FontSize = (float)spanning.FontSize;
            textRange.CharacterFormat.FontName = spanning.FontFamily.Source;
            FontWeight fontWeight = spanning.FontWeight;
            if (fontWeight.ToString() == "Bold")
                textRange.CharacterFormat.Bold = true;
            return textRange;
        }

        #endregion

        #region Helper Methods

        internal static WParagraph GetParagraphformatIndentSpacing(ParagraphAdv paragraph, WParagraph currentParagraph)
        {
            if (paragraph != null)
            {
                currentParagraph.ParagraphFormat.LeftIndent = (float)(paragraph.LeftIndent * 0.75);
                currentParagraph.ParagraphFormat.RightIndent = (float)(paragraph.RightIndent * 0.75);
                currentParagraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                currentParagraph.ParagraphFormat.LineSpacing = (float)(paragraph.LineSpacing * 12);
                currentParagraph.ParagraphFormat.BeforeSpacing = (float)(paragraph.BeforeSpacing * 0.75);
                currentParagraph.ParagraphFormat.AfterSpacing = (float)(paragraph.AfterSpacing * 0.75);
                currentParagraph.ParagraphFormat.HorizontalAlignment = GetParagraphTextAlignment(paragraph.TextAlignment);
            }
            return currentParagraph;
        }

        internal static SubSuperScript GetSubSuperScript(Baseline baseline)
        {
            SubSuperScript localsubsuper = SubSuperScript.None;
            switch (baseline)
            {
                case Baseline.Normal:
                    break;
                case Baseline.Superscript:
                    localsubsuper = SubSuperScript.SuperScript;
                    break;
                case Baseline.Subscript:
                    localsubsuper = SubSuperScript.SubScript;
                    break;
                default:
                    break;
            }
            return localsubsuper;
        }

        /// <summary>
        /// Get the Horizontal alignment based upon the TextAlignment
        /// </summary>
        /// <param name="align"></param>
        /// <returns></returns>
        internal static DocIO.DLS.HorizontalAlignment GetParagraphTextAlignment(TextAlignment align)
        {
            DocIO.DLS.HorizontalAlignment currentalign = DocIO.DLS.HorizontalAlignment.Left;
            switch (align)
            {
                case TextAlignment.Center:
                    currentalign = DocIO.DLS.HorizontalAlignment.Center;
                    break;
                case TextAlignment.Justify:
                    currentalign = DocIO.DLS.HorizontalAlignment.Justify;
                    break;
                case TextAlignment.Left:
                    currentalign = DocIO.DLS.HorizontalAlignment.Left;
                    break;
                case TextAlignment.Right:
                    currentalign = DocIO.DLS.HorizontalAlignment.Right;
                    break;
                default:
                    break;
            }
            return currentalign;
        }

        /// <summary>
        /// Save the Document as Docx format
        /// </summary>
        /// <param name="saveDocument"></param>
        /// <param name="stream"></param>
        internal static void SaveAsDocx(WordDocument saveDocument, Stream stream)
        {
            saveDocument.Save(stream, FormatType.Docx);
        }

        /// <summary>
        /// Save the Document as the Doc format.
        /// </summary>
        /// <param name="saveDocument"></param>
        /// <param name="stream"></param>
        internal static void SaveAsDoc(WordDocument saveDocument, Stream stream)
        {
            saveDocument.Save(stream, FormatType.Doc);
        }

        /// <summary>
        /// Convert the color into the correct format
        /// </summary>
        /// <param name="convertingColor"></param>
        /// <returns></returns>
        internal static System.Drawing.Color ConvertHighlightColor(Color convertingColor)
        {
            switch (convertingColor.ToString().ToLower())
            {
                case "#ffff0000":
                    return System.Drawing.Color.Red;
                case "#ffffff00":
                    return System.Drawing.Color.Yellow;
                case "#ff00ff00":
                    return System.Drawing.Color.Lime;
                case "#ff8b0000":
                    return System.Drawing.Color.DarkRed;
                case "#ff00008b":
                    return System.Drawing.Color.DarkBlue;
                case "#ff808000":
                    return System.Drawing.Color.Olive;
                case "#ff40e0d0":
                    return System.Drawing.Color.Turquoise;
                case "#ff808080":
                    return System.Drawing.Color.Gray;
                case "#ffff00ff":
                    return System.Drawing.Color.Magenta;
                case "#ffc0c0c0":
                    return System.Drawing.Color.Silver;
                case "#ff0000ff":
                    return System.Drawing.Color.Blue;
                case "#ff800080":
                    return System.Drawing.Color.Purple;
                case "#ff000000":
                    return System.Drawing.Color.Black;
                case "#00000000":
                    return System.Drawing.Color.Empty;
                default:
                    return System.Drawing.Color.FromArgb(convertingColor.A, convertingColor.R, convertingColor.G, convertingColor.B);
            }
        }

        internal static System.Drawing.Color Convert(Color convertingColor)
        {
            Color brush = ((Color)convertingColor);
            System.Drawing.Color colr = System.Drawing.Color.FromArgb(brush.A, brush.R, brush.G, brush.B);
            if (brush.A == 0 && brush.B == 0 && brush.G == 0 && brush.R == 0)
            {
                return System.Drawing.Color.Empty;
            }
            return colr;
        }

        #endregion
    }
}
