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
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using System.Reflection;
using System.Collections.Generic;
#if WPF
using System.Drawing.Imaging;
#endif

namespace Syncfusion.Windows.Tools.Controls
{
    public class DocxImporting
    {
        #region Private Members

        private static bool IsStarted = false;
        static HyperlinkAdv hyperlinkadv = null;
        private static SectionAdv wordSection = null;
        private static string m_format = string.Empty;
        private static List<WTableCell> m_cellsToRemove = new List<WTableCell>();

        #endregion

        #region Constructor

        public DocxImporting()
        {

        }
        #endregion

        #region Implementation

        /// <summary>
        /// Converts the Word document stream to DocumentAdv instance.
        /// </summary>
        /// <param name="documentStream">The document stream.</param>
        /// <param name="documentExtension">The word document extension (".doc" or ".docx").</param>
        /// <returns></returns>
        public static DocumentAdv ConvertToDocumentAdv(Stream documentStream, string documentExtension)
        {
            WordDocument doc = null;
            m_format = documentExtension;
            if (documentStream != null)
            {
                if (documentExtension == ".docx")
                    doc = new WordDocument(documentStream, FormatType.Docx);
                else if (documentExtension == ".doc")
                    doc = new WordDocument(documentStream, FormatType.Doc);
            }
            if (doc != null)
                return ParseStream(doc);
            return null;
        }

        /// <summary>
        /// Returns the DocumentAdv from WordDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        internal static DocumentAdv ParseStream(WordDocument doc)
        {
            DocumentAdv document = new DocumentAdv();
            try
            {
                wordSection = new SectionAdv();
                document.Sections.Add(wordSection);
                for (int s =0; s< doc.Sections.Count ; s++)
                {
                    IWSection section = doc.Sections[s];
                    Size size = new Size(
                        (section.PageSetup.PageSize.Width *96)/72 ,
                        (section.PageSetup.PageSize.Height * 96) / 72);
                    wordSection.PageSize = size;
                    wordSection.PageContentMargin = new Thickness(
                        (section.PageSetup.Margins.Left * 96) / 72, 
                        (section.PageSetup.Margins.Top * 96) / 72, 
                        (section.PageSetup.Margins.Right * 96) / 72,
                        (section.PageSetup.Margins.Bottom * 96)/72);
                                            
                    for(int k =0 ; k < section.Body.ChildEntities.Count ; k++)
                    {
                        Entity item = section.Body.ChildEntities[k];
                        switch (item.EntityType)
                        {
                            case EntityType.Paragraph:
                                GetParagrpahAdvfromParagraphs(item as WParagraph, wordSection);
                                break;
                            case EntityType.Table:
                                TableAdv tempTable = GetTableAdv(item as WTable);
                                if (tempTable != null)
                                {
                                    wordSection.Blocks.Add(tempTable);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                new InvalidOperationException("UnSupported Document");
            }

            return document;
        }

        /// <summary>
        /// Get the paragraphAdv if it founds the entity type as Paragraphs
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        /// <summary>
        /// Get the paragraphAdv if it founds the entity type as Paragraphs
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        internal static void GetParagrpahAdvfromParagraphs(WParagraph paragraph, SectionAdv section)
        {
            ParagraphAdv richTextParagraph = new ParagraphAdv();
            for(int i=0 ; i < paragraph.Items.Count; i++)
            {
                ParagraphItem paragraphItem = paragraph.Items[i];
                richTextParagraph.ListType = ConvertDocIOListType(paragraph.ListFormat.ListType);
                GetPargraphAdv(paragraphItem, richTextParagraph);
                GetParagraphIndentSpacing(paragraph, richTextParagraph);
            }
            section.Blocks.Add(richTextParagraph);
        }

        /// <summary>
        /// Get the ParagraphAdv if it founds the Table as the Entity type.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        internal static void GetParagraphAdvfromTables(WTable table, SectionAdv inputsection)
        {
            ParagraphAdv tableparagraph = null;
            foreach (WTableRow row in table.Rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    tableparagraph = new ParagraphAdv();
                    foreach (WParagraph para in cell.Paragraphs)
                    {
                        foreach (ParagraphItem item in para.Items)
                        {
                            GetPargraphAdv(item, tableparagraph);
                            GetTableIndentSpacing(table, tableparagraph);
                        }
                    }
                    inputsection.Blocks.Add(tableparagraph);
                }
            }
        }

        /// <summary>
        /// Gets the PargraphAdv.
        /// </summary>
        /// <param name="paragraphitem">The paragraphitem.</param>
        /// <param name="paragraph">The paragraph.</param>
        internal static void GetPargraphAdv(ParagraphItem paragraphitem, ParagraphAdv paragraph)
        {
            switch (paragraphitem.EntityType)
            {
                case EntityType.FieldMark:
                    if ((paragraphitem as WFieldMark).Type == FieldMarkType.FieldSeparator)
                        IsStarted = true;
                    else if ((paragraphitem as WFieldMark).Type == FieldMarkType.FieldEnd)
                        IsStarted = false;
                    break;
                case EntityType.Field:
                    hyperlinkadv = new HyperlinkAdv();
                    WField field = paragraphitem as WField;
                    if (field.FieldType == FieldType.FieldHyperlink)
                    {
                        Syncfusion.DocIO.DLS.Hyperlink hyp = new DocIO.DLS.Hyperlink(field);
                        bool Ok = false;
                        string tempstr = field.FieldValue.Replace("\"", string.Empty);
                        if (hyp.Type == HyperlinkType.WebLink)
                        {
                            Ok = tempstr.Contains("http://") || tempstr.Contains("https://");
                        }
                        else
                        {
                            Ok = tempstr.Contains("mailto:");
                        }

                        if (Ok)
                        {
                            if (Uri.IsWellFormedUriString(hyp.Uri, UriKind.RelativeOrAbsolute))
                            {
                                hyperlinkadv.NavigationUrl = hyp.Uri;
                            }
                        }
                    }
                    break;

                case EntityType.Picture:
                    BitmapImage bitmap;
                    WPicture picture = paragraphitem as WPicture;
                    ImageContainerAdv image = new ImageContainerAdv();
#if !WPF
                    bitmap = ConvertByteArrayToImage(picture.ImageBytes);
                    image.ImageSource = bitmap;
#else
                    ImageFormat imgformat = GetImageFormat(picture.Image);
                    if (imgformat == ImageFormat.Png)
                    {
                        bitmap = ConvertByteArrayToImage(picture.ImageBytes);
                        image.ImageSource = bitmap;
                    }
                    else
                    {
                        byte[] btyeArr = picture.ImageBytes;
                        image.ImageBytes = btyeArr;
                        var bmp = new BitmapImage();
#if WPF
                        bmp.BeginInit();
#endif
                        bmp.SetSource(new MemoryStream(btyeArr));
#if WPF
                        bmp.EndInit();
#endif
                        image.ImageSource = bmp;
                        image.Width = bmp.PixelWidth;
                        image.Height = bmp.PixelHeight;
                    }
#endif
                    image.Width = picture.Width * (picture.WidthScale / 100) * (96 / 72);
                    image.Height = picture.Height * (picture.HeightScale / 100) * (96 / 72);
                    image.ImageBytes = picture.ImageBytes;
                    paragraph.Inlines.Add(image);
                    break;

                case EntityType.TextRange:
                    WTextRange textRange = paragraphitem as WTextRange;
                    SpanAdv spanAdv = new SpanAdv();
                    if (!IsStarted)
                    {
                        spanAdv.Text = textRange.Text;
                        WParagraph owner = textRange.OwnerParagraph as WParagraph;
                        WCharacterFormat format = textRange.CharacterFormat;
                        GetformattedSpanAdv(owner, format, spanAdv);
                        paragraph.Inlines.Add(spanAdv);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(hyperlinkadv.Text))
                            hyperlinkadv = new HyperlinkAdv();
                        hyperlinkadv.Text = textRange.Text;
                        GetFormattedHyperlinkAdv(textRange.CharacterFormat, hyperlinkadv);
                        paragraph.Inlines.Add(hyperlinkadv);
                    }
                    break;
                default:
                    break;
            }
        }

#if WPF
        private static ImageFormat GetImageFormat(System.Drawing.Image img)
        {
            ImageFormat format = ImageFormat.Jpeg;
            string value = img.RawFormat.Guid.ToString();

            if (value == ImageFormat.Emf.Guid.ToString() || value == ImageFormat.Wmf.Guid.ToString())
            {
                format = ImageFormat.Png;
            }
            else if (value == ImageFormat.Png.Guid.ToString())
            {
                format = ImageFormat.Png;
            }
            else
                format = ImageFormat.Jpeg;

            return format;
        }
#endif

        private static TableAdv GetTableAdv(WTable wTable)
        {
            TableAdv table = null;
            TableRowAdv tablerow = null;
            TableCellAdv tablecell = null;
            List<BlockAdv> blocks = new List<BlockAdv>();
            List<TableCellAdv> m_removed = new List<TableCellAdv>();
            IWTable wTab = wTable as IWTable;
            RowFormat rowformat = new RowFormat();
            table = new TableAdv();
            table.LeftIndent = rowformat.LeftIndent;

            try
            {
                foreach (WTableRow row in wTab.Rows)
                {
                    double height = row.Height;
                    tablerow = new TableRowAdv();

                    WCellCollection cells = row.Cells;

                    foreach (WTableCell c in cells)
                    {
                        tablecell = new TableCellAdv();

                        if (m_format == ".docx")
                        {
                            if (!c.CellFormat.BackColor.IsEmpty)
                            {
                                tablecell.Background = Color.FromArgb(c.CellFormat.BackColor.A, c.CellFormat.BackColor.R, c.CellFormat.BackColor.G, c.CellFormat.BackColor.B);
                            }
                        }

                        if (m_format == ".docx")
                        {
                            tablecell.ColumnSpan = c.GridSpan;
                        }
                        else if (m_format == ".doc")
                        {

                        }

                        tablecell.RowSpan = GetRowSpan(c, wTable);
                        tablecell.Blocks = GetBlocksFromTableCell(c);
                        foreach (BlockAdv b2 in GetBlocksFromSpannedCells(c, wTable))
                        {
                            tablecell.Blocks.Add(b2);
                        }
                        if (c.CellFormat.VerticalMerge == CellMerge.Continue)
                        {
                            m_removed.Add(tablecell);
                        }
                        tablerow.Cells.Add(tablecell);
                    }
                    table.Rows.Add(tablerow);
                }


                foreach (WTableCell wCl in m_cellsToRemove)
                {
                    foreach (WTableRow wRw in wTable.Rows)
                    {
                        int i = 0;
                        while (i < wRw.Cells.Count)
                        {
                            WTableCell wCell = wRw.Cells[i];
                            if (wCl == wCell)
                            {
                                wRw.Cells.Remove(wCell);
                                continue;
                            }
                            i++;
                        }
                    }
                }

                foreach (TableCellAdv cell4 in m_removed)
                {
                    foreach (TableRowAdv row3 in table.Rows)
                    {
                        int j = 0;
                        while (j < row3.Cells.Count)
                        {
                            TableCellAdv cell5 = row3.Cells[j];
                            if (cell5 == cell4)
                            {
                                row3.Cells.Remove(cell5);
                            }
                            j++;
                        }
                    }

                }
            }
            catch { }

            return table;
        }

        internal static int GetRowSpan(WTableCell wCell,WTable wTable)
        {
            int rowspan = 1;

            if (wCell.CellFormat.VerticalMerge == CellMerge.Start)
            {
                int index = wCell.GetCellIndex();
                int rowindex = wCell.OwnerRow.GetRowIndex();
                bool started = false;

                foreach (WTableRow wRow in wTable.Rows)
                {
                    if (started)
                    {
                        WTableCell wCl = wRow.Cells[index];
                        if (wCl.CellFormat.VerticalMerge == CellMerge.Continue)
                        {
                            rowspan++;
                            //wRow.Cells.Remove(wCl);
                        }
                        else if (wCl.CellFormat.VerticalMerge == CellMerge.None)
                        {
                            break;
                        }
                        else if (wCl.CellFormat.VerticalMerge == CellMerge.Start)
                        {
                            break;
                        }
                    }
                    if (wRow.GetRowIndex() == rowindex)
                    {
                        started = true;
                    }
                }
            }

            return rowspan;
        }

        private static BlockCollection<BlockAdv> GetBlocksFromSpannedCells(WTableCell wCel,WTable wTable)
        {
            BlockCollection<BlockAdv> blks = new BlockCollection<BlockAdv>();

            if (wCel.CellFormat.VerticalMerge == CellMerge.Start)
            {
                int index = wCel.GetCellIndex();
                int rowindex = wCel.OwnerRow.GetRowIndex();
                bool started = false;

                foreach (WTableRow wRow in wTable.Rows)
                {
                    if (started)
                    {
                        WTableCell wCl = wRow.Cells[index];
                        if (wCl.CellFormat.VerticalMerge == CellMerge.Continue)
                        {
                            foreach (BlockAdv b in GetBlocksFromTableCell(wCl))
                            {
                                blks.Add(b);
                            }
                           //wRow.Cells.Remove(wCl);
                            m_cellsToRemove.Add(wCl);
                        }
                        else if (wCl.CellFormat.VerticalMerge == CellMerge.None)
                        {
                            break;
                        }
                    }
                    if (wRow.GetRowIndex() == rowindex)
                    {
                        started = true;
                    }
                }
            }
            return blks;
        }

        private static BlockCollection<BlockAdv> GetBlocksFromTableCell(WTableCell wCell)
        {
            BlockCollection<BlockAdv> blocks = new BlockCollection<BlockAdv>();
            foreach (Entity entity in wCell.ChildEntities)
            {
                switch (entity.EntityType)
                {
                    case EntityType.Paragraph:
                        ParagraphAdv richTextParagraph = new ParagraphAdv();
                        WParagraph paragraph = entity as WParagraph;
                        foreach (ParagraphItem paragraphItem in paragraph.Items)
                        {
                            richTextParagraph.ListType = ConvertDocIOListType(paragraph.ListFormat.ListType);
                            GetPargraphAdv(paragraphItem, richTextParagraph);
                            GetParagraphIndentSpacing(paragraph, richTextParagraph);
                        }
                        blocks.Add(richTextParagraph);
                        break;
                    case EntityType.Table:
                        TableAdv tempTable = GetTableAdv(entity as WTable);
                        if (tempTable != null)
                        {
                            blocks.Add(tempTable);
                        }
                        break;
                    default:
                        break;
                }
            }
            return blocks;
        }

        private static ListType ConvertDocIOListType(Syncfusion.DocIO.DLS.ListType listType)
        {
            ListType result = ListType.None;
            switch (listType)
            {
                case Syncfusion.DocIO.DLS.ListType.Numbered:
                    result = ListType.Numbered; ;
                    break;
                case Syncfusion.DocIO.DLS.ListType.Bulleted:
                    result = ListType.Bulleted;
                    break;
                case Syncfusion.DocIO.DLS.ListType.NoList:
                    result = ListType.None;
                    break;
                default:
                    break;
            }
            return result;
        }
        
        /// <summary>
        /// Get the Paragraph's Left, Right indent and Before,After spacing
        /// </summary>
        /// <param name="wordparagraph"></param>
        /// <param name="local_paragraph"></param>
        /// <returns></returns>
        internal static void GetParagraphIndentSpacing(WParagraph wordparagraph, ParagraphAdv local_paragraph)
        {
            if (wordparagraph.ParagraphFormat != null)
            {
                local_paragraph.LeftIndent = Convert.ToDouble(wordparagraph.ParagraphFormat.LeftIndent * 96 / 72);
                local_paragraph.RightIndent = Convert.ToDouble(wordparagraph.ParagraphFormat.RightIndent * 96 / 72);
                local_paragraph.LineSpacing = LineSpaceConverter(wordparagraph.ParagraphFormat.LineSpacing);
                local_paragraph.BeforeSpacing = Convert.ToDouble(wordparagraph.ParagraphFormat.BeforeSpacing * 96 / 72);
                local_paragraph.AfterSpacing = Convert.ToDouble(wordparagraph.ParagraphFormat.AfterSpacing * 96 / 72);
                local_paragraph.TextAlignment = GetParagraphAlignment(wordparagraph.ParagraphFormat.HorizontalAlignment);
            }
        }

        /// <summary>
        /// Get the Table's Left, Right indent and Before,After spacing
        /// </summary>
        /// <param name="wordtable"></param>
        /// <param name="local_paragraph"></param>
        /// <returns></returns>
        internal static void GetTableIndentSpacing(WTable wordtable, ParagraphAdv local_paragraph)
        {
            if (wordtable.TableFormat != null)
            {
                local_paragraph.LeftIndent = Convert.ToDouble(wordtable.TableFormat.LeftIndent);
                local_paragraph.LineSpacing = LineSpaceConverter(wordtable.TableFormat.CellSpacing);
                //local_paragraph.BeforeSpacing = Convert.ToDouble(wordtable.TableFormat.CellSpacing);
                //local_paragraph.AfterSpacing = Convert.ToDouble(wordtable.TableFormat.CellSpacing);
                local_paragraph.TextAlignment = GetParagraphAlignment(wordtable.TableFormat.HorizontalAlignment);
            }
        }

        /// <summary>
        /// Get the formatted the SpanAdv input span
        /// </summary>
        /// <param name="format"></param>
        /// <param name="spanAdv"></param>
        /// <returns></returns>
        internal static void GetformattedSpanAdv(WParagraph Wparagraph, WCharacterFormat format, SpanAdv spanAdv)
        {
            spanAdv.Baseline = GetBaseLine(format.SubSuperScript);
            spanAdv.StrikeThrough = GetStrikeThrough(format);
            if (format.HighlightColor != null)
                if (format.HighlightColor.IsEmpty == false  && !(format.HighlightColor.A==255 && format.HighlightColor.B==255 && format.HighlightColor.G==255 && format.HighlightColor.R==255))
                {
                    System.Drawing.Color highlightColor = format.HighlightColor;
                    spanAdv.HighlightColor = Color.FromArgb(highlightColor.A, highlightColor.R, highlightColor.G, highlightColor.B);
                }
            if (format.TextColor != null)
            {
                System.Drawing.Color textColor = format.TextColor;
                if (textColor.IsEmpty)
                {
                    textColor = System.Drawing.Color.Black;
                }
                spanAdv.Foreground = Color.FromArgb(textColor.A, textColor.R, textColor.G, textColor.B);
            }
            if (format.UnderlineStyle == UnderlineStyle.Single)
                spanAdv.Underline = true;

            spanAdv.FontSize = Convert.ToDouble(format.FontSize);
            spanAdv.FontFamily = new FontFamily(format.FontName);
            spanAdv.FontWeight = GetFontWeight(format);
            spanAdv.FontStyle = GetFontStyle(format);
        }

        /// <summary>
        /// Get the formatted HyperlinkAdv
        /// </summary>
        /// <param name="format"></param>
        /// <param name="hyperlink"></param>
        /// <returns></returns>
        internal static void GetFormattedHyperlinkAdv(WCharacterFormat format, HyperlinkAdv hyperlink)
        {
            hyperlink.Baseline = GetBaseLine(format.SubSuperScript);
            hyperlink.StrikeThrough = GetStrikeThrough(format);
            if (format.TextColor == System.Drawing.Color.Black || format.TextColor.Name == "ff000000"
                || format.TextColor.IsEmpty)
            {
                hyperlink.Foreground = Colors.Blue;
            }
            else
            {
                System.Drawing.Color textColor = format.TextColor;
                hyperlink.Foreground = Color.FromArgb(textColor.A, textColor.R, textColor.G, textColor.B);
            }
            if (format.HighlightColor != null)
                if (format.HighlightColor.IsEmpty == false && !(format.HighlightColor.A == 255 && format.HighlightColor.B == 255 && format.HighlightColor.G == 255 && format.HighlightColor.R == 255))
                {
                    System.Drawing.Color highlightColor = format.HighlightColor;
                    hyperlink.HighlightColor = Color.FromArgb(highlightColor.A, highlightColor.R, highlightColor.G, highlightColor.B);
                }
            if (format.UnderlineStyle == UnderlineStyle.Single)
                hyperlink.Underline = true;

            hyperlink.FontSize = Convert.ToDouble(format.FontSize);
            hyperlink.FontFamily = new FontFamily(format.FontName);
            hyperlink.FontWeight = GetFontWeight(format);
            hyperlink.FontStyle = GetFontStyle(format);
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Get the strike through for paragraph from the character format.
        /// </summary>
        /// <param name="charcterformat"></param>
        /// <returns></returns>
        internal static StrikeThrough GetStrikeThrough(WCharacterFormat charcterformat)
        {
            StrikeThrough localvalue = StrikeThrough.None;
            if (charcterformat.Strikeout)
                localvalue = StrikeThrough.SingleStrike;
            else if (charcterformat.DoubleStrike)
                localvalue = StrikeThrough.DoubleStrike;
            else
                localvalue = StrikeThrough.None;
            return localvalue;
        }

        /// <summary>
        /// Get the FontWeight from the given charcterformat.
        /// </summary>
        /// <param name="charformat"></param>
        /// <returns></returns>
        internal static FontWeight GetFontWeight(WCharacterFormat charformat)
        {
            FontWeight localfontweight = FontWeights.Normal;
            if (charformat.Bold)
                localfontweight = FontWeights.Bold;
            else
                localfontweight = FontWeights.Normal;
            return localfontweight;
        }

        /// <summary>
        /// Get the FontStyle from the given charcter format.
        /// </summary>
        /// <param name="fontstyleformat"></param>
        /// <returns></returns>
        internal static FontStyle GetFontStyle(WCharacterFormat fontstyleformat)
        {
            FontStyle localfontstyle = FontStyles.Normal;
            if (fontstyleformat.Italic)
                localfontstyle = FontStyles.Italic;
            else
                localfontstyle = FontStyles.Normal;
            return localfontstyle;
        }

        /// <summary>
        /// Returns the Paragraph's TextAlignment in the RTE
        /// </summary>
        /// <param name="horizontalalign"></param>
        /// <returns></returns>
        internal static TextAlignment GetParagraphAlignment(DocIO.DLS.HorizontalAlignment horizontalalign)
        {
            TextAlignment textalign = TextAlignment.Left;
            switch (horizontalalign)
            {
                case Syncfusion.DocIO.DLS.HorizontalAlignment.Center:
                    textalign = TextAlignment.Center;
                    break;
                case Syncfusion.DocIO.DLS.HorizontalAlignment.Justify:
                    textalign = TextAlignment.Justify;
                    break;
                case Syncfusion.DocIO.DLS.HorizontalAlignment.Left:
                    textalign = TextAlignment.Left;
                    break;
                case Syncfusion.DocIO.DLS.HorizontalAlignment.Right:
                    textalign = TextAlignment.Right;
                    break;
                default:
                    break;
            }
            return textalign;
        }

        /// <summary>
        /// Get the Paragraphs alignment from the RowAlignment
        /// </summary>
        /// <param name="horizontalalign"></param>
        /// <returns></returns>
        internal static TextAlignment GetParagraphAlignment(RowAlignment horizontalalign)
        {
            TextAlignment textalign = TextAlignment.Left;
            switch (horizontalalign)
            {
                case RowAlignment.Center:
                    textalign = TextAlignment.Center;
                    break;
                case RowAlignment.Left:
                    textalign = TextAlignment.Left;
                    break;
                case RowAlignment.Right:
                    textalign = TextAlignment.Right;
                    break;
                default:
                    break;
            }
            return textalign;
        }

        /// <summary>
        /// Gets the Sub or Super script format.
        /// </summary>
        /// <param name="baseline"></param>
        /// <returns></returns>
        private static Baseline GetBaseLine(SubSuperScript baseline)
        {
            Baseline subsuper = Baseline.Normal;
            switch (baseline)
            {
                case SubSuperScript.None:
                    subsuper = Baseline.Normal;
                    break;
                case SubSuperScript.SubScript:
                    subsuper = Baseline.Subscript;
                    break;
                case SubSuperScript.SuperScript:
                    subsuper = Baseline.Superscript;
                    break;
                default:
                    break;
            }
            return subsuper;
        }

        /// <summary>
        /// Converts the byte[] to Image
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        internal static BitmapImage ConvertByteArrayToImage(byte[] byteArray)
        {
            BitmapImage bitmap = null;
            if (byteArray != null)
            {
                MemoryStream ms = new MemoryStream(byteArray, 0, byteArray.Length);
                ms.Write(byteArray, 0, byteArray.Length);
                bitmap = new BitmapImage();
#if WPF
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();
#else               
                bitmap.SetSource(ms);
#endif
                return bitmap;
            }
            return null;
        }

#if WPF
        internal static BitmapImage ConvertBitmapToBitmapImage(WPicture wPicture)
        {
            System.Drawing.Image image = wPicture.Image;
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();
            return bitmap;
        }
#endif
        /// <summary>
        /// Line space converter.
        /// </summary>
        /// <param name="lineSpace"></param>
        /// <returns></returns>
        internal static double LineSpaceConverter(float lineSpace)
        {
            double rtbLineSpace = 0;
            if (lineSpace == 0.0)
            {
                rtbLineSpace = 1.0;
            }
            else
            {
                rtbLineSpace = lineSpace / 12.0;
            }
            return rtbLineSpace;
        }

        #endregion
    }
}
