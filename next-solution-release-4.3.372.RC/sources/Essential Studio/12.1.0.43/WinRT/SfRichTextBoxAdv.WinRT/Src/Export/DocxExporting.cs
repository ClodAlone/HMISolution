#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows.Input;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using System.IO;
using System.Windows;
#if WPF
using Color = System.Drawing.Color;
using UIColor = System.Windows.Media.Color;
using System.Drawing;
#else
using Color = Syncfusion.DocIO.DLS.Color;
using UIColor = Windows.UI.Color;
using Windows.UI.Text;
using Windows.UI.Xaml;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class DocxExporting
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DocxExporting"/> class.
        /// </summary>
        internal DocxExporting()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Return the Document from the DocumentAdv in RTE
        /// </summary>
        /// <param name="richText"></param>
        /// <param name="fileStream"></param>
        /// <param name="type"></param>
        internal static void ConvertToDocument(DocumentAdv documentadv, Stream fileStream, FormatType formatType)
        {
            WordDocument wordDocument = GetWordDocument(documentadv);
            switch (formatType)
            {
                case FormatType.Doc:
                    wordDocument.Save(fileStream, DocIO.FormatType.Doc);
                    break;
                case FormatType.Docx:
                    wordDocument.Save(fileStream, DocIO.FormatType.Word2013);
                    break;
                case FormatType.Txt:
                    wordDocument.Save(fileStream, DocIO.FormatType.Txt);
                    break;
                default:
                    wordDocument.Save(fileStream, DocIO.FormatType.Rtf);
                    break;
            }
        }
        /// <summary>
        /// Get Word document
        /// </summary>
        /// <param name="documentAdv"></param>
        /// <returns></returns>
        internal static WordDocument GetWordDocument(DocumentAdv documentAdv)
        {
            WordDocument wordDocument = new WordDocument();
            for (int s = 0; s < documentAdv.Sections.Count; s++)
            {
                SectionAdv sectionAdv = documentAdv.Sections[s];
                IWSection section = null;
                WHeadersFooters headerFooters = null;
#if !WPF
                UIDispatcher.Execute(() =>
                    {
#endif
                        if (s == 0)
                        {
                            wordDocument.DefaultTabWidth = (float)(documentAdv.DefaultTabWidth * 72) / 96;
                            wordDocument.DefParaFormat = new WParagraphFormat(wordDocument);
                            SerializeParagraphFormat(documentAdv.ParagraphFormat, wordDocument.DefParaFormat);
                            wordDocument.DefCharFormat = new WCharacterFormat(wordDocument);
                            SerializeCharacterFormat(documentAdv.CharacterFormat, wordDocument.DefCharFormat);
                            SerializeBackground(documentAdv, wordDocument);
                            SerializeList(documentAdv, wordDocument);
                        }
                        section = new WSection(wordDocument);
                        SerializeSectionFormat(sectionAdv, section);
                        headerFooters = section.HeadersFooters;
                        if (sectionAdv.HeaderFooters != null)
                            SerializeHeaderFooters(sectionAdv.HeaderFooters, headerFooters);
                        wordDocument.Sections.Add(section);
                        SerializeTextBody(sectionAdv.Blocks, section.Body);
#if !WPF
                    });
#endif
            }
            return wordDocument;
        }
        /// <summary>
        /// Serializes the background.
        /// </summary>
        /// <param name="documentAdv">The document adv.</param>
        /// <param name="wDocument">The word document.</param>
        private static void SerializeBackground(DocumentAdv documentAdv, WordDocument wDocument)
        {
            wDocument.Background.Color = Convert(documentAdv.BackgroundColor);
        }
        /// <summary>
        /// Serializes the list.
        /// </summary>
        /// <param name="documentAdv">The document adv.</param>
        /// <param name="wDocument">The w document.</param>
        private static void SerializeList(DocumentAdv documentAdv, WordDocument wDocument)
        {
            //Serializes the abstract list.
            for (int i = 0; i < documentAdv.Lists.Count; i++)
            {
                ListAdv listAdv = documentAdv.Lists[i];
                if (wDocument.ListStyles.FindByName(listAdv.Name) != null)
                    continue;
                ListStyle listStyle = new ListStyle(wDocument);
                listStyle.Name = listAdv.Name;
                wDocument.ListStyles.Add(listStyle);
                listStyle.ListType = ConvertListType(listAdv.AbstractList.ListType);
                SerializeListLevels(listAdv.AbstractList, listStyle);
            }
        }
        /// <summary>
        /// Serializes the list.
        /// </summary>
        /// <param name="wDocument">The wdocument.</param>
        /// <param name="listAdv">The list adv.</param>
        private static void SerializeList(WordDocument wDocument, ListAdv listAdv)
        {
            if (wDocument.ListStyles.FindByName(listAdv.Name) != null)
                return;
            ListStyle listStyle = new ListStyle(wDocument);
            listStyle.Name = listAdv.Name;
            wDocument.ListStyles.Add(listStyle);
            listStyle.ListType = ConvertListType(listAdv.AbstractList.ListType);
            SerializeListLevels(listAdv.AbstractList, listStyle);
        }
        /// <summary>
        /// Converts the type of the list.
        /// </summary>
        /// <param name="listType">Type of the list.</param>
        /// <returns></returns>
        private static DocIO.DLS.ListType ConvertListType(ListType listType)
        {
            DocIO.DLS.ListType result = DocIO.DLS.ListType.NoList;
            switch (listType)
            {
                case ListType.Numbering:
                    result = DocIO.DLS.ListType.Numbered;
                    break;
                case ListType.Bullet:
                    result = DocIO.DLS.ListType.Bulleted;
                    break;
            }
            return result;
        }
        /// <summary>
        /// Serializes the list levels.
        /// </summary>
        /// <param name="abstractListAdv">The abstract list adv.</param>
        /// <param name="listStyle">The list style.</param>
        private static void SerializeListLevels(AbstractListAdv abstractListAdv, ListStyle listStyle)
        {
            //Serializes the list levels.
            for (int i = 0; i < abstractListAdv.Levels.InnerList.Count; i++)
            {
                ListLevelAdv listLevelAdv = abstractListAdv.Levels[i];
                WListLevel listLevel = new WListLevel(listStyle);
                listStyle.Levels.Add(listLevel);
                if (listLevelAdv.ListLevelPattern == ListLevelPattern.Bullet)
                {
                    listLevel.PatternType = ListPatternType.Bullet;
                    listLevel.BulletCharacter = listLevelAdv.BulletCharacter;
                }
                else
                {
                    listLevel.PatternType = (ListPatternType)((byte)listLevelAdv.ListLevelPattern);
                    listLevel.StartAt = listLevelAdv.StartAt;
                    listLevel.NumberSufix = ".";
                }
                SerializeCharacterFormat(listLevelAdv.CharacterFormat, listLevel.CharacterFormat);
                SerializeParagraphFormat(listLevelAdv.ParagraphFormat, listLevel.ParagraphFormat);
            }
        }
        /// <summary>
        /// Serialize Header and Footer
        /// </summary>
        /// <param name="headerFooters"></param>
        /// <param name="wHeaderFooters"></param>
        internal static void SerializeHeaderFooters(HeaderFooters headerFooters, WHeadersFooters wHeaderFooters)
        {
            SerializeTextBody(headerFooters.Header.Blocks, wHeaderFooters.Header);
            SerializeTextBody(headerFooters.Footer.Blocks, wHeaderFooters.Footer);
            SerializeTextBody(headerFooters.EvenHeader.Blocks, wHeaderFooters.EvenHeader);
            SerializeTextBody(headerFooters.EvenFooter.Blocks, wHeaderFooters.EvenFooter);
            SerializeTextBody(headerFooters.FirstPageHeader.Blocks, wHeaderFooters.FirstPageHeader);
            SerializeTextBody(headerFooters.FirstPageFooter.Blocks, wHeaderFooters.FirstPageFooter);
        }
        /// <summary>
        /// Serialize TextBody
        /// </summary>
        /// <param name="blockAdvCollection"></param>
        /// <param name="textBody"></param>
        private static void SerializeTextBody(BlockAdvCollection blockAdvCollection, WTextBody textBody)
        {
            foreach (BlockAdv block in blockAdvCollection)
            {
                if (block is ParagraphAdv)
                {
                    SerializeParagraph(block as ParagraphAdv, textBody);
                }
                else if (block is TableAdv)
                {
                    SerializeTable(block as TableAdv, textBody);
                }
            }
        }
        #region serialize paragraph
        /// <summary>
        /// Serialize Paragraph
        /// </summary>
        /// <param name="paragraphAdv"></param>
        /// <param name="textBody"></param>
        internal static void SerializeParagraph(ParagraphAdv paragraphAdv, WTextBody textBody)
        {
            WParagraph paragraph = null;
#if !WPF
            UIDispatcher.Execute(() =>
                {
#endif
                    paragraph = new WParagraph(textBody.Document);
                    if (!string.IsNullOrEmpty(paragraphAdv.ParagraphFormat.ListFormat.ListId)
                        && textBody.Document.ListStyles.FindByName(paragraphAdv.ParagraphFormat.ListFormat.ListId) != null)
                    {
                        paragraph.ListFormat.ApplyStyle(paragraphAdv.ParagraphFormat.ListFormat.ListId);
                        paragraph.ListFormat.ListLevelNumber = paragraphAdv.ParagraphFormat.ListFormat.ListLevelNumber;
                    }
                    SerializeParagraphFormat(paragraphAdv.ParagraphFormat, paragraph.ParagraphFormat);
                    SerializeCharacterFormat(paragraphAdv.CharacterFormat, paragraph.BreakCharacterFormat);
#if !WPF
                });
#endif
            for (int i = 0; i < paragraphAdv.Inlines.Count; i++)
            {
                Inline inline = paragraphAdv.Inlines[i];
                SerializePargraphItems(inline, paragraph, false);
            }
            if (paragraph != null)
                textBody.ChildEntities.Add(paragraph);
        }
        /// <summary>
        /// Serialize Paragraph items
        /// </summary>
        /// <param name="inline"></param>
        /// <param name="paragraph"></param>
        internal static void SerializePargraphItems(Inline inline, WParagraph paragraph, bool isCopyingToClipboard)
        {
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                if (inline is SpanAdv)
                {
                    SpanAdv span = inline as SpanAdv;
                    if (!string.IsNullOrEmpty(span.Text))
                    {
                        if (paragraph.Items.Count > 0 && paragraph.LastItem is WField && string.IsNullOrEmpty((paragraph.LastItem as WField).FieldCode))
                            (paragraph.LastItem as WField).FieldCode = span.Text;
                        else
                        {
                            IWTextRange textRange = paragraph.AppendText(span.Text);
                            SerializeCharacterFormat(inline.CharacterFormat, textRange.CharacterFormat);
                        }
                    }
                }
                else if (inline is FieldCharacterAdv && (isCopyingToClipboard || (inline as FieldCharacterAdv).IsLinkedFieldCharacter()))
                {
                    if (inline is FieldBeginAdv)
                    {
                        WField field = new WField(paragraph.Document);
                        paragraph.Items.Add(field);
                        SerializeCharacterFormat(inline.CharacterFormat, field.CharacterFormat);
                    }
                    else
                    {
                        WFieldMark fieldMark = paragraph.AppendFieldMark(inline is FieldSeparatorAdv ? FieldMarkType.FieldSeparator : FieldMarkType.FieldEnd);
                        SerializeCharacterFormat(inline.CharacterFormat, fieldMark.CharacterFormat);
                    }
                }
                else if (inline is ImageContainerAdv)
                {
                    ImageContainerAdv imagecontainer = inline as ImageContainerAdv;
                    if (imagecontainer.ImageBytes != null)
                    {
                        IWPicture picture = paragraph.AppendPicture(imagecontainer.ImageBytes);
                        picture.Width = (float)imagecontainer.Width * ((float)72 / 96);
                        picture.Height = (float)imagecontainer.Height * ((float)72 / 96);
                    }
                }
                else if (inline is UIContainerAdv)
                {
                    // UIContainerAdv uicontainer = inline as UIContainerAdv;
                }
#if !WPF
            });
#endif
        }
        #endregion

        #region SerializeSectionFormat
        /// <summary>
        /// Serialize section format
        /// </summary>
        /// <param name="sectionAdv"></param>
        /// <param name="section"></param>
        internal static void SerializeSectionFormat(SectionAdv sectionAdv, IWSection section)
        {
            section.PageSetup.PageSize = new SizeF(
               (float)(sectionAdv.SectionFormat.PageSize.Width * 72) / 96,
               (float)(sectionAdv.SectionFormat.PageSize.Height * 72) / 96);
            section.PageSetup.Margins = new MarginsF(
                (float)(sectionAdv.SectionFormat.PageMargin.Left * 72) / 96,
                (float)(sectionAdv.SectionFormat.PageMargin.Top * 72) / 96,
                (float)(sectionAdv.SectionFormat.PageMargin.Right * 72) / 96,
                (float)(sectionAdv.SectionFormat.PageMargin.Bottom * 72) / 96);
            section.PageSetup.HeaderDistance = (float)(sectionAdv.SectionFormat.HeaderDistance * 72) / 96;
            section.PageSetup.FooterDistance = (float)(sectionAdv.SectionFormat.FooterDistance * 72) / 96;
            section.PageSetup.DifferentFirstPage = sectionAdv.SectionFormat.DifferentFirstPage;
            section.PageSetup.DifferentOddAndEvenPages = sectionAdv.SectionFormat.DifferentOddAndEvenPages;
        }
        #endregion

        #region Serialize ParagraphFormat
        /// <summary>
        /// Serialize paragraph format
        /// </summary>
        /// <param name="paragraphFormat">The paragraph format.</param>
        /// <param name="wparagraphFormat">The wparagraph format.</param>
        internal static void SerializeParagraphFormat(ParagraphFormat paragraphFormat, WParagraphFormat wparagraphFormat)
        {
            if (paragraphFormat != null)
            {
                WParagraph paragraph = wparagraphFormat.OwnerBase as WParagraph;
                if (paragraph != null && paragraph.Document != null && !string.IsNullOrEmpty(paragraphFormat.ListFormat.ListId)
                    && paragraphFormat.Document != null)
                {
                    ListAdv listAdv = paragraphFormat.Document.GetListAdv(paragraphFormat.ListFormat.ListId);
                    if (listAdv != null)
                    {
                        if (paragraph.Document.ListStyles.FindByName(listAdv.Name) == null)
                            SerializeList(paragraph.Document, listAdv);
                        paragraph.ListFormat.ApplyStyle(paragraphFormat.ListFormat.ListId);
                        paragraph.ListFormat.ListLevelNumber = paragraphFormat.ListFormat.ListLevelNumber;
                    }
                }
                wparagraphFormat.LeftIndent = (float)(paragraphFormat.LeftIndent * 72) / 96;
                wparagraphFormat.RightIndent = (float)(paragraphFormat.RightIndent * 72) / 96;
                wparagraphFormat.FirstLineIndent = (float)(paragraphFormat.FirstLineIndent * 72) / 96;
                SerializeLineSpace(paragraphFormat, wparagraphFormat);
                wparagraphFormat.BeforeSpacing = (float)(paragraphFormat.BeforeSpacing * 72) / 96;
                wparagraphFormat.AfterSpacing = (float)(paragraphFormat.AfterSpacing * 72) / 96;
                wparagraphFormat.HorizontalAlignment = GetParagraphAlignment(paragraphFormat.TextAlignment);
            }
        }
        /// <summary>
        /// Returns the Paragraph's HorizontalAlignment
        /// </summary>
        /// <param name="horizontalalign"></param>
        /// <returns></returns>
        internal static DocIO.DLS.HorizontalAlignment GetParagraphAlignment(TextAlignment textAlign)
        {
            DocIO.DLS.HorizontalAlignment horizAlign = DocIO.DLS.HorizontalAlignment.Left;
            switch (textAlign)
            {
                case TextAlignment.Center:
                    horizAlign = DocIO.DLS.HorizontalAlignment.Center;
                    break;
                case TextAlignment.Justify:
                    horizAlign = DocIO.DLS.HorizontalAlignment.Justify;
                    break;
                case TextAlignment.Left:
                    horizAlign = DocIO.DLS.HorizontalAlignment.Left;
                    break;
                case TextAlignment.Right:
                    horizAlign = DocIO.DLS.HorizontalAlignment.Right;
                    break;
                default:
                    break;
            }
            return horizAlign;
        }
        /// <summary>
        /// Serialize line space
        /// </summary>
        /// <param name="lineSpace"></param>
        /// <returns></returns>
        internal static void SerializeLineSpace(ParagraphFormat srcFormat, WParagraphFormat destFormat)
        {
            if (srcFormat.LineSpacing < 0)
            {
                destFormat.LineSpacing = (float)-(srcFormat.LineSpacing * 72) / 96;
                destFormat.LineSpacingRule = DocIO.LineSpacingRule.Exactly;
            }
            else
            {
                switch (srcFormat.LineSpacingType)
                {
                    case LineSpacingType.AtLeast:
                        destFormat.LineSpacingRule = DocIO.LineSpacingRule.AtLeast;
                        destFormat.LineSpacing = (float)(srcFormat.LineSpacing * 72) / 96;
                        break;
                    case LineSpacingType.Exactly:
                        destFormat.LineSpacingRule = DocIO.LineSpacingRule.Exactly;
                        destFormat.LineSpacing = (float)(srcFormat.LineSpacing * 72) / 96;
                        break;
                    default:
                        destFormat.LineSpacingRule = DocIO.LineSpacingRule.Multiple;
                        destFormat.LineSpacing = (float)srcFormat.LineSpacing * 12f;
                        break;
                }
            }
        }
        #endregion

        #region Serialize CharacterFormat
        /// <summary>
        /// Serialize the Character format
        /// </summary>
        /// <param name="characterFormat">The character format.</param>
        /// <param name="wcharacterFormat">The wcharacter format.</param>
        internal static void SerializeCharacterFormat(CharacterFormat characterFormat, WCharacterFormat wcharacterFormat)
        {
            if (characterFormat == null || wcharacterFormat == null)
                return;
            if (characterFormat.ReadLocalValue(CharacterFormat.BaselineAlignmentProperty) is BaselineAlignment)
                SetSubSuperScript(characterFormat.BaselineAlignment, wcharacterFormat);
            if (characterFormat.ReadLocalValue(CharacterFormat.StrikeThroughProperty) is StrikeThrough)
                SetStrikeThrough(characterFormat.StrikeThrough, wcharacterFormat);
            if (characterFormat.HighlightColor != HighlightColor.NoColor)
                wcharacterFormat.HighlightColor = ConvertHighlightColor(characterFormat.GetHighlightColor());
            if (characterFormat.FontColor != null)
            {
                Color textColor = Convert(characterFormat.FontColor);
                wcharacterFormat.TextColor = textColor;
            }
            if (characterFormat.Underline != Underline.None)
                wcharacterFormat.UnderlineStyle = (UnderlineStyle)((byte)characterFormat.Underline);

            wcharacterFormat.FontSize = (float)(characterFormat.FontSize * 72) / 96;
            wcharacterFormat.FontName = characterFormat.FontFamily.Source;
            wcharacterFormat.Bold = characterFormat.Bold;
            wcharacterFormat.Italic = characterFormat.Italic;
        }
        /// <summary>
        /// Sets SubSuperScript
        /// </summary>
        /// <param name="baselineAlignment">The baseline alignment.</param>
        /// <param name="wcharacterFormat">The wcharacter format.</param>
        private static void SetSubSuperScript(BaselineAlignment baselineAlignment, WCharacterFormat wcharacterFormat)
        {
            switch (baselineAlignment)
            {
                case BaselineAlignment.Superscript:
                    wcharacterFormat.SubSuperScript = SubSuperScript.SuperScript;
                    break;
                case BaselineAlignment.Subscript:
                    wcharacterFormat.SubSuperScript = SubSuperScript.SubScript;
                    break;
                default:
                    wcharacterFormat.SubSuperScript = SubSuperScript.None;
                    break;
            }
        }
        /// <summary>
        /// Sets the strike through.
        /// </summary>
        /// <param name="strike">The strike.</param>
        /// <param name="wcharacterFormat">The wcharacter format.</param>
        private static void SetStrikeThrough(StrikeThrough strike, WCharacterFormat wcharacterFormat)
        {
            switch (strike)
            {
                case StrikeThrough.SingleStrike:
                    wcharacterFormat.Strikeout = true;
                    break;
                case StrikeThrough.DoubleStrike:
                    wcharacterFormat.DoubleStrike = true;
                    break;
                default:
                    wcharacterFormat.Strikeout = false;
                    wcharacterFormat.DoubleStrike = false;
                    break;
            }
        }
        /// <summary>
        /// Convert the color into the correct format
        /// </summary>
        /// <param name="convertingColor"></param>
        /// <returns></returns>
        internal static Color ConvertHighlightColor(UIColor convertingColor)
        {
            switch (convertingColor.ToString().ToLower())
            {
                case "#ffffff00":
                    return Color.Yellow;
                case "#ff00ff00":
                    return Color.Green;
                case "#ff00ffff":
                    return Color.Cyan;
                case "#ffff00ff":
                    return Color.Magenta;
                case "#ff0000ff":
                    return Color.Blue;
                case "#ffff0000":
                    return Color.Red;
                case "#ff000080":
                    return Color.DarkBlue;
                case "#ff008080":
                    return Color.DarkCyan;
                case "#ff008000":
                    return Color.DarkGreen;
                case "#ff800080":
                    return Color.DarkMagenta;
                case "#ff800000":
                    return Color.DarkRed;
                case "#ff808000":
                    return Color.Gold;
                case "#ff808080":
                    return Color.DarkGray;
                case "#ffc0c0c0":
                    return Color.LightGray;
                case "#ff000000":
                    return Color.Black;
            }
            return Color.Empty;
        }

        /// <summary>
        /// Converts the specified UI color.
        /// </summary>
        /// <param name="uiColor">The UIColor.</param>
        /// <returns></returns>
        internal static Color Convert(UIColor uiColor)
        {
            return Color.FromArgb(uiColor.A, uiColor.R, uiColor.G, uiColor.B);
        }
        #endregion

        #region serialize Table
        /// <summary>
        /// Serialize Table
        /// </summary>
        /// <param name="tableAdv"></param>
        /// <param name="textBody"></param>
        internal static void SerializeTable(TableAdv tableAdv, WTextBody textBody)
        {
            WTable table = new WTable(textBody.Document);
            table.ResetCells(tableAdv.Rows.Count, tableAdv.TableHolder.Columns.Count);
            for (int i = 0; i < tableAdv.Rows.Count; i++)
            {
                TableRowAdv row = tableAdv.Rows[i];
                WTableRow wRow = table.Rows[i];
                WTableCell wCell = null;
                int columnIndex = 0;
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    TableCellAdv cell = row.Cells[j];
                    if (j == 0)
                        columnIndex = cell.ColumnIndex;
                    wCell = wRow.Cells[cell.ColumnIndex];
                    columnIndex++;
                    if (columnIndex < cell.ColumnIndex + cell.CellFormat.ColumnSpan)
                    {
                        //Updates horizontal merge for cells with column span greater than 1.
                        wCell.CellFormat.HorizontalMerge = CellMerge.Start;
                        while (columnIndex < cell.ColumnIndex + cell.CellFormat.ColumnSpan)
                        {
                            WTableCell wTableCell = wRow.Cells[columnIndex];
                            wTableCell.CellFormat.HorizontalMerge = CellMerge.Continue;
                            columnIndex++;
                        }
                    }
                    int rowSpan = cell.CellFormat.RowSpan;
                    if (rowSpan > 1)
                    {
                        //Updates vertical merge for cells with row span greater than 1.
                        wCell.CellFormat.VerticalMerge = CellMerge.Start;
                        for (int k = 1; k < rowSpan; k++)
                        {
                            WTableRow wTableRow = table.Rows[i + k];
                            WTableCell wTableCell = wTableRow.Cells[cell.ColumnIndex];
                            wTableCell.CellFormat.VerticalMerge = CellMerge.Continue;
                        }
                    }
                    SerializeTableCellFormat(cell, wCell);
                    wCell.Width = (float)(cell.CellFormat.CellWidth * 72) / 96;
                    SerializeTextBody(cell.Blocks, wCell as WTextBody);
                    //Updates horizontal merge of vertical merged cells at end of row.
                    if (j == row.Cells.Count - 1 && columnIndex < wRow.Cells.Count)
                    {
                        WTableCell wTableCell = wRow.Cells[columnIndex];
                        wTableCell.CellFormat.HorizontalMerge = CellMerge.Start;
                        columnIndex++;
                        while (columnIndex < wRow.Cells.Count)
                        {
                            wTableCell = wRow.Cells[columnIndex];
                            wTableCell.CellFormat.HorizontalMerge = CellMerge.Continue;
                            columnIndex++;
                        }
                    }
                }
            }
            table.TableFormat.IsAutoResized = true;
            textBody.ChildEntities.Add(table);
        }
        /// <summary>
        /// Serializes the table cell format.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="wCell">The w cell.</param>
        private static void SerializeTableCellFormat(TableCellAdv cell, WTableCell wCell)
        {
            wCell.CellFormat.SamePaddingsAsTable = false;
            wCell.CellFormat.Paddings.Left = (float)(cell.CellFormat.CellMargin.Left * 72) / 96;
            wCell.CellFormat.Paddings.Right = (float)(cell.CellFormat.CellMargin.Right * 72) / 96;
            wCell.CellFormat.Paddings.Top = (float)(cell.CellFormat.CellMargin.Top * 72) / 96;
            wCell.CellFormat.Paddings.Bottom = (float)(cell.CellFormat.CellMargin.Bottom * 72) / 96;
            if (cell.CellFormat.Background != UIColor.FromArgb(0, 0, 0, 0))
                wCell.CellFormat.BackColor = Convert(cell.CellFormat.Background);
            wCell.CellFormat.Borders.Color = Convert(cell.OwnerTable.BorderBrush);
            if (cell.OwnerTable.BorderThickness == 0.0)
                wCell.CellFormat.Borders.LineWidth = 0.0f;
        }
        /// <summary>
        /// Serializes the table row.
        /// </summary>
        /// <param name="tableRowAdv">The table row adv.</param>
        /// <param name="wTable">The wtable.</param>
        internal static void SerializeTableRow(TableRowAdv tableRowAdv, WTable wTable)
        {
            WTableRow wRow = new WTableRow(wTable.Document);
            foreach (TableCellAdv cell in tableRowAdv.Cells)
            {
                SerializeTableCell(cell, wRow);
            }
            wTable.Rows.Add(wRow);
        }
        /// <summary>
        /// Serializes the table cell.
        /// </summary>
        /// <param name="tableCellAdv">The table cell adv.</param>
        /// <param name="wTableRow">The wtable row.</param>
        internal static void SerializeTableCell(TableCellAdv tableCellAdv, WTableRow wTableRow)
        {
            WTableCell wCell = new WTableCell(wTableRow.Document);
            SerializeTableCellFormat(tableCellAdv, wCell);
            wCell.Width = (float)(tableCellAdv.CellFormat.CellWidth * 72) / 96;
            SerializeTextBody(tableCellAdv.Blocks, wCell as WTextBody);
            wTableRow.Cells.Add(wCell);
        }
        #endregion
        #endregion
    }
}
