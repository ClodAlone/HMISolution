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
using System.Windows.Input;
using System.IO;
using Syncfusion.DocIO;
using System.Reflection;
using System.Collections.Generic;
using Syncfusion.DocIO.DLS;
using WListType = Syncfusion.DocIO.DLS.ListType;
#if WPF
using Color = System.Drawing.Color;
using UIColor = System.Windows.Media.Color;
using Size = System.Windows.Size;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#else
using Color = Syncfusion.DocIO.DLS.Color;
using UIColor = Windows.UI.Color;
using Size = Windows.Foundation.Size;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.System.Threading;
#endif

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class DocxImporting
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DocxImporting"/> class.
        /// </summary>
        internal DocxImporting()
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Converts to document adv.
        /// </summary>
        /// <param name="rte">The rte.</param>
        /// <param name="wordDocument">The word document.</param>
        internal static void ConvertToDocumentAdv(SfRichTextBoxAdv rte, WordDocument wordDocument)
        {
            if (wordDocument != null)
                ParseDocument(rte, wordDocument);
        }
        /// <summary>
        /// Parses the document.
        /// </summary>
        /// <param name="rte">The rte.</param>
        /// <param name="doc">The doc.</param>
        private static void ParseDocument(SfRichTextBoxAdv rte, WordDocument doc)
        {
            DocumentAdv document = null;
            for (int s = 0; s < doc.Sections.Count; s++)
            {
                IWSection section = doc.Sections[s];
                SectionAdv sectionAdv = null;
                HeaderFooters headerFooters = null;
#if !WPF
                UIDispatcher.Execute(() =>
                    {
#endif
                        if (s == 0)
                        {
                            document = new DocumentAdv();
                            document.DefaultTabWidth = (doc.DefaultTabWidth * 96) / 72;
                            ParseParagraphFormat(doc.DefParaFormat, document.ParagraphFormat);
                            ParseCharacterFormat(doc.DefCharFormat, document.CharacterFormat);
                            ParseBackground(doc, document);
                            ParseList(doc, document);
                        }
                        sectionAdv = new SectionAdv();
                        ParseSectionFormat(section, sectionAdv);
                        headerFooters = sectionAdv.HeaderFooters;
#if !WPF
                    });
#endif
                if (!section.HeadersFooters.LinkToPrevious || !section.HeadersFooters.IsEmpty)
                    ParseHeaderFooters(section.HeadersFooters, headerFooters);
                document.Sections.Add(sectionAdv);
                if (s == 0)
#if WPF
                    rte.Document = document;
#else
                    UIDispatcher.Execute(() => rte.Document = document);
#endif
                ParseTextBody(section.Body, sectionAdv.Blocks);
            }
            doc.Close();
        }
        /// <summary>
        /// Parses the background.
        /// </summary>
        /// <param name="wDocument">The word document.</param>
        /// <param name="documentAdv">The document adv.</param>
        private static void ParseBackground(WordDocument wDocument, DocumentAdv documentAdv)
        {
            Color backgroundColor = wDocument.Background.Color;
            if (!backgroundColor.IsEmpty)
                documentAdv.BackgroundColor = UIColor.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G, backgroundColor.B);
        }
        /// <summary>
        /// Parses the list.
        /// </summary>
        /// <param name="wDocument">The w document.</param>
        /// <param name="documentAdv">The document adv.</param>
        internal static void ParseList(WordDocument wDocument, DocumentAdv documentAdv)
        {
            //Parses the abstract list.
            for (int i = 0; i < wDocument.ListStyles.Count; i++)
            {
                ListStyle listStyle = wDocument.ListStyles[i];
                if (documentAdv.GetListAdv(listStyle.Name) == null)
                {
                    AbstractListAdv abstractListAdv = new AbstractListAdv(documentAdv);
                    documentAdv.AbstractLists.Add(abstractListAdv);
                    abstractListAdv.ListType = ConvertDocIOListType(listStyle.ListType);
                    ParseListLevels(listStyle, abstractListAdv);
                    ListAdv listAdv = new ListAdv(documentAdv);
                    listAdv.Name = listStyle.Name;
                    listAdv.AbstractList = abstractListAdv;
                    documentAdv.Lists.Add(listAdv);
                }
            }
        }
        /// <summary>
        /// Parses the list levels.
        /// </summary>
        /// <param name="listStyle">The list style.</param>
        /// <param name="abstractListAdv">The abstract list adv.</param>
        private static void ParseListLevels(ListStyle listStyle, AbstractListAdv abstractListAdv)
        {
            //Parses the list levels.
            for (int i = 0; i < listStyle.Levels.Count; i++)
            {
                WListLevel listLevel = listStyle.Levels[i];
                ListLevelAdv listLevelAdv = abstractListAdv.AddListLevel();
                if (listLevel.PatternType == ListPatternType.Bullet)
                {
                    listLevelAdv.ListLevelPattern = ListLevelPattern.Bullet;
                    listLevelAdv.BulletCharacter = listLevel.BulletCharacter;
                }
                else
                {
                    listLevelAdv.ListLevelPattern = (ListLevelPattern)((byte)listLevel.PatternType);
                    listLevelAdv.StartAt = listLevel.StartAt;
                }
                ParseCharacterFormat(listLevel.CharacterFormat, listLevelAdv.CharacterFormat);
                ParseParagraphFormat(listLevel.ParagraphFormat, listLevelAdv.ParagraphFormat);
            }
        }
        internal static void ParseSectionFormat(IWSection section, SectionAdv sectionAdv)
        {
            sectionAdv.SectionFormat.PageSize = new Size(
                (section.PageSetup.PageSize.Width * 96) / 72,
                (section.PageSetup.PageSize.Height * 96) / 72);
            sectionAdv.SectionFormat.PageMargin = new Thickness(
                (section.PageSetup.Margins.Left * 96) / 72,
                (section.PageSetup.Margins.Top * 96) / 72,
                (section.PageSetup.Margins.Right * 96) / 72,
                (section.PageSetup.Margins.Bottom * 96) / 72);
            if (section.PageSetup.HeaderDistance != -0.05f)
                sectionAdv.SectionFormat.HeaderDistance = (section.PageSetup.HeaderDistance * 96) / 72;
            if (section.PageSetup.FooterDistance != -0.05f)
                sectionAdv.SectionFormat.FooterDistance = (section.PageSetup.FooterDistance * 96) / 72;
            sectionAdv.SectionFormat.DifferentFirstPage = section.PageSetup.DifferentFirstPage;
            sectionAdv.SectionFormat.DifferentOddAndEvenPages = section.PageSetup.DifferentOddAndEvenPages;
        }
        internal static void ParseHeaderFooters(WHeadersFooters wHeadersFooters, HeaderFooters headerFooters)
        {
            ParseTextBody(wHeadersFooters.Header, headerFooters.Header.Blocks);
            ParseTextBody(wHeadersFooters.Footer, headerFooters.Footer.Blocks);
            ParseTextBody(wHeadersFooters.EvenHeader, headerFooters.EvenHeader.Blocks);
            ParseTextBody(wHeadersFooters.EvenFooter, headerFooters.EvenFooter.Blocks);
            ParseTextBody(wHeadersFooters.FirstPageHeader, headerFooters.FirstPageHeader.Blocks);
            ParseTextBody(wHeadersFooters.FirstPageFooter, headerFooters.FirstPageFooter.Blocks);
        }
        private static void ParseTextBody(WTextBody textBody, BlockAdvCollection blockAdvCollection)
        {
            for (int i = 0; i < textBody.ChildEntities.Count; i++)
            {
                Entity item = textBody.ChildEntities[i];
                switch (item.EntityType)
                {
                    case EntityType.Paragraph:
                        ParseParagraph(item as WParagraph, blockAdvCollection);
                        break;
                    case EntityType.Table:
                        ParseTable(item as WTable, blockAdvCollection);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Get the paragraphAdv if it founds the entity type as Paragraphs
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        internal static void ParseParagraph(WParagraph paragraph, BlockAdvCollection blockAdvCollection)
        {
            ParagraphAdv paragraphAdv = ParseParagraph(paragraph);
            if (paragraphAdv != null)
                blockAdvCollection.Add(paragraphAdv);
        }
        /// <summary>
        /// Parses the paragraph.
        /// </summary>
        /// <param name="wParagraph">The wparagraph.</param>
        /// <returns></returns>
        internal static ParagraphAdv ParseParagraph(WParagraph wParagraph)
        {
            ParagraphAdv paragraphAdv = null;
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                paragraphAdv = new ParagraphAdv();
                if (wParagraph.ListFormat.ListType != WListType.NoList
                    && wParagraph.ListFormat.CurrentListStyle != null)
                {
                    paragraphAdv.ParagraphFormat.ListFormat.ListId = wParagraph.ListFormat.CustomStyleName;
                    paragraphAdv.ParagraphFormat.ListFormat.ListLevelNumber = wParagraph.ListFormat.ListLevelNumber;
                }
                ParseParagraphFormat(wParagraph.ParagraphFormat, paragraphAdv.ParagraphFormat);
                ParseCharacterFormat(wParagraph.BreakCharacterFormat, paragraphAdv.CharacterFormat);
#if !WPF
            });
#endif
            for (int i = 0; i < wParagraph.Items.Count; i++)
            {
                ParagraphItem paragraphItem = wParagraph.Items[i];
                ParsePargraphItems(paragraphItem, paragraphAdv);
            }
            return paragraphAdv;
        }

        /// <summary>
        /// Gets the PargraphAdv.
        /// </summary>
        /// <param name="paragraphitem">The paragraphitem.</param>
        /// <param name="paragraph">The paragraph.</param>
        internal static void ParsePargraphItems(ParagraphItem paragraphitem, ParagraphAdv paragraph)
        {
            switch (paragraphitem.EntityType)
            {
                case EntityType.FieldMark:
                    WFieldMark fieldMark = paragraphitem as WFieldMark;
                    if (fieldMark.Type == FieldMarkType.FieldSeparator)
                    {
                        FieldSeparatorAdv fieldSeparator = null;
#if WPF
                        fieldSeparator = new FieldSeparatorAdv();
#else
                        UIDispatcher.Execute(() => fieldSeparator = new FieldSeparatorAdv());
#endif
                        paragraph.Inlines.Add(fieldSeparator);
                    }
                    else
                    {
                        FieldEndAdv fieldEnd = null;
#if WPF
                        fieldEnd = new FieldEndAdv();
#else
                        UIDispatcher.Execute(() => fieldEnd = new FieldEndAdv());
#endif
                        paragraph.Inlines.Add(fieldEnd);
                    }
                    break;
                case EntityType.Field:
                    WField field = paragraphitem as WField;
                    FieldBeginAdv fieldBegin = null;
#if WPF
                    fieldBegin = new FieldBeginAdv();
#else
                    UIDispatcher.Execute(() => fieldBegin = new FieldBeginAdv());
#endif
                    if (field.FieldEnd != null)
                        fieldBegin.HasFieldEnd = true;
                    paragraph.Inlines.Add(fieldBegin);
                    string fieldCode = field.FieldCode;
                    if (field.FieldType == FieldType.FieldHyperlink && string.IsNullOrEmpty(fieldCode))
                    {
                        fieldCode = " HYPERLINK ";
                        if (!string.IsNullOrEmpty(field.FormattingString))
                            fieldCode += field.FormattingString + " ";
                        if (!string.IsNullOrEmpty(field.FieldValue))
                            fieldCode += field.FieldValue;
                    }
                    AddSpanAdv(paragraph, fieldCode, field.CharacterFormat);
                    break;
                case EntityType.Picture:
                    WPicture picture = paragraphitem as WPicture;
                    ImageContainerAdv image = null;
#if !WPF
                    UIDispatcher.Execute(() =>
                        {
#endif
                            image = new ImageContainerAdv();
                            BitmapImage bitmap = ConvertByteArrayToImage(picture.ImageBytes);
                            image.ImageSource = bitmap;
                            image.Width = picture.Width * (picture.WidthScale / 100) * ((double)96 / 72);
                            image.Height = picture.Height * (picture.HeightScale / 100) * ((double)96 / 72);
                            image.ImageBytes = picture.ImageBytes;
#if !WPF
                        });
#endif
                    image.IsInlineImage = picture.TextWrappingStyle == TextWrappingStyle.Inline;
                    paragraph.Inlines.Add(image);
                    break;
                case EntityType.TextRange:
                    WTextRange textRange = paragraphitem as WTextRange;
                    AddSpanAdv(paragraph, textRange.Text, textRange.CharacterFormat);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Adds the span adv.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="text">The text.</param>
        /// <param name="wCharacterFormat">The wcharacter format.</param>
        private static void AddSpanAdv(ParagraphAdv paragraph, string text, WCharacterFormat wCharacterFormat)
        {
            if (!string.IsNullOrEmpty(text))
            {
                SpanAdv span = null;
#if !WPF
                UIDispatcher.Execute(() =>
                {
#endif
                    span = new SpanAdv();
                    span.Text = text;
                    ParseCharacterFormat(wCharacterFormat, span.CharacterFormat);
#if !WPF
                });
#endif
                paragraph.Inlines.Add(span);
            }
        }
        /// <summary>
        /// Parses the table.
        /// </summary>
        /// <param name="wTable">The wtable.</param>
        /// <param name="blockAdvCollection">The blockadv collection.</param>
        private static void ParseTable(WTable wTable, BlockAdvCollection blockAdvCollection)
        {
            TableAdv table = ParseTable(wTable);
            if (table != null)
                blockAdvCollection.Add(table);
        }
        /// <summary>
        /// Parses the table.
        /// </summary>
        /// <param name="wTable">The wtable.</param>
        /// <returns></returns>
        internal static TableAdv ParseTable(WTable wTable)
        {
            TableAdv table = null;
            TableRowAdv tablerow = null;
            TableCellAdv tablecell = null;
            wTable.ApplyBaseStyleFormats();
#if !WPF
            UIDispatcher.Execute(() =>
            {
#endif
                table = new TableAdv();
                table.TableFormat.LeftIndent = (wTable.IndentFromLeft * 96) / 72;
                table.TableFormat.Background = UIColor.FromArgb(wTable.TableFormat.BackColor.A, wTable.TableFormat.BackColor.R, wTable.TableFormat.BackColor.G, wTable.TableFormat.BackColor.B);
                foreach (WTableRow row in wTable.Rows)
                {
                    double height = row.Height;
                    tablerow = new TableRowAdv();
                    WCellCollection cells = row.Cells;
                    foreach (WTableCell c in cells)
                    {
                        if (c.CellFormat.VerticalMerge == CellMerge.Continue)
                            continue;
                        tablecell = new TableCellAdv();
                        tablecell.CellFormat.CellWidth = (c.Width * 96) / 72;
                        tablecell.CellFormat.ColumnSpan = c.GridSpan;
                        if (!c.CellFormat.BackColor.IsEmpty)
                            tablecell.CellFormat.Background = UIColor.FromArgb(c.CellFormat.BackColor.A, c.CellFormat.BackColor.R, c.CellFormat.BackColor.G, c.CellFormat.BackColor.B);
                        Paddings paddings = c.CellFormat.Paddings;
                        if (c.CellFormat.SamePaddingsAsTable)
                            paddings = wTable.TableFormat.Paddings;
                        tablecell.CellFormat.CellMargin = new Thickness((paddings.Left * 96) / 72, (paddings.Top * 96) / 72, (paddings.Right * 96) / 72, (paddings.Bottom * 96) / 72);
                        tablecell.CellFormat.RowSpan = GetRowSpan(c, wTable);
                        ParseTextBody(c, tablecell.Blocks);
                        tablerow.Cells.Add(tablecell);
                    }
                    table.Rows.Add(tablerow);
                }
#if !WPF
            });
#endif
            return table;
        }

        /// <summary>
        /// Gets the row span.
        /// </summary>
        /// <param name="wCell">The wcell.</param>
        /// <param name="wTable">The wtable.</param>
        /// <returns></returns>
        internal static int GetRowSpan(WTableCell wCell, WTable wTable)
        {
            int rowspan = 1;
            if (wCell.OwnerRow.NextSibling != null && wCell.CellFormat.VerticalMerge == CellMerge.Start)
            {
                WTableRow row = wCell.OwnerRow;
                int gridBefore = row.RowFormat.GridBefore;
                foreach (WTableCell cell in row.Cells)
                {
                    if (cell == wCell)
                        break;
                    gridBefore += cell.GridSpan;
                }
                int rowindex = row.GetRowIndex();
                for (int i = rowindex + 1; i < wTable.Rows.Count; i++)
                {
                    WTableRow nextRow = wTable.Rows[i];
                    int nextRowGridBefore = nextRow.RowFormat.GridBefore;
                    WTableCell adjacentCell = null;
                    foreach (WTableCell cell in nextRow.Cells)
                    {
                        if (gridBefore == nextRowGridBefore)
                        {
                            adjacentCell = cell;
                            break;
                        }
                        nextRowGridBefore += cell.GridSpan;
                    }
                    if (adjacentCell != null && adjacentCell.CellFormat.VerticalMerge == CellMerge.Continue)
                        rowspan++;
                    else
                        break;
                }
            }
            return rowspan;
        }
        private static ListType ConvertDocIOListType(WListType listType)
        {
            ListType result = ListType.None;
            switch (listType)
            {
                case WListType.Numbered:
                    result = ListType.Numbering;
                    break;
                case WListType.Bulleted:
                    result = ListType.Bullet;
                    break;
            }
            return result;
        }
        
        /// <summary>
        /// Get the Paragraph's Left, Right indent and Before,After spacing
        /// </summary>
        /// <param name="wordparagraph"></param>
        /// <param name="paragraphAdv"></param>
        /// <returns></returns>
        private static void ParseParagraphFormat(WParagraphFormat wparagraphFormat, ParagraphFormat paragraphFormat)
        {
            if (wparagraphFormat != null)
            {
                paragraphFormat.LeftIndent = (wparagraphFormat.LeftIndent * 96) / 72;
                paragraphFormat.RightIndent = (wparagraphFormat.RightIndent * 96) / 72;
                paragraphFormat.FirstLineIndent = (wparagraphFormat.FirstLineIndent * 96) / 72;
                ParseLineSpace(wparagraphFormat, paragraphFormat);
                paragraphFormat.BeforeSpacing = (wparagraphFormat.BeforeSpacing * 96) / 72;
                paragraphFormat.AfterSpacing = (wparagraphFormat.AfterSpacing * 96) / 72;
                paragraphFormat.TextAlignment = GetParagraphAlignment(wparagraphFormat.HorizontalAlignment);
            }
        }

        /// <summary>
        /// Get the formatted the SpanAdv input span
        /// </summary>
        /// <param name="wcharacterformat"></param>
        /// <param name="inline"></param>
        /// <returns></returns>
        internal static void ParseCharacterFormat(WCharacterFormat wcharacterformat, CharacterFormat characterFormat)
        {
            if (wcharacterformat == null)
                return;
            characterFormat.BaselineAlignment = GetBaseLine(wcharacterformat.SubSuperScript);
            characterFormat.StrikeThrough = GetStrikeThrough(wcharacterformat);
            if (!wcharacterformat.HighlightColor.IsEmpty 
                && !(wcharacterformat.HighlightColor.A == 255 && wcharacterformat.HighlightColor.B == 255 && wcharacterformat.HighlightColor.G == 255 && wcharacterformat.HighlightColor.R == 255))
                    characterFormat.HighlightColor = GetHighlightColor(wcharacterformat.HighlightColor);
            if (wcharacterformat.TextColor != null)
            {
                Color textColor = wcharacterformat.TextColor;
                if (!textColor.IsEmpty)
                characterFormat.FontColor = UIColor.FromArgb(textColor.A, textColor.R, textColor.G, textColor.B);
            }
            if (wcharacterformat.UnderlineStyle != UnderlineStyle.None
                && wcharacterformat.UnderlineStyle != (UnderlineStyle)5)
                characterFormat.Underline = (Underline)((byte)wcharacterformat.UnderlineStyle);

            characterFormat.FontSize = ((wcharacterformat.FontSize == 0 ? 0.5 : wcharacterformat.FontSize) * 96) / 72;
            characterFormat.FontFamily = new FontFamily(wcharacterformat.FontName);
            characterFormat.Bold = wcharacterformat.Bold;
            characterFormat.Italic = wcharacterformat.Italic;
        }
        private static HighlightColor GetHighlightColor(Color highlightColor)
        {
            switch (highlightColor.Name.ToLower())
            {
                case "yellow":
                case "ffff00":
                case "ffffff00":
                    return HighlightColor.Yellow;
                case "green":
                case "ff008000":
                case "008000":
                    return HighlightColor.BrightGreen;
                case "cyan":
                case "ff00ffff":
                case "00ffff":
                    return HighlightColor.Turquoise;
                case "magenta":
                case "ffff00ff":
                case "ff00ff":
                    return HighlightColor.Pink;
                case "blue":
                case "ff0000ff":
                case "0000ff":
                    return HighlightColor.Blue;
                case "red":
                case "ffff0000":
                case "ff0000":
                    return HighlightColor.Red;
                case "darkblue":
                case "ff00008b":
                case "00008b":
                    return HighlightColor.DarkBlue;
                case "darkcyan":
                case "008b8b":
                case "ff008b8b":
                    return HighlightColor.Teal;
                case "darkgreen":
                case "ff006400":
                case "006400":
                    return HighlightColor.Green;
                case "darkmagenta":
                case "ff8b008b":
                case "8b008b":
                    return HighlightColor.Violet;
                case "darkred":
                case "ff8b0000":
                case "8b0000":
                    return HighlightColor.DarkRed;
                case "gold":
                case "ffffd700":
                case "ffd700":
                    return HighlightColor.DarkYellow;
                case "darkgray":
                case "ffa9a9a9":
                case "a9a9a9":
                case "ff808080":
                case "808080":
                    return HighlightColor.Gray50;
                case "lightgray":
                case "ffd3d3d3":
                case "d3d3d3":
                    return HighlightColor.Gray25;
                case "black":
                case "ff000000":
                case "000000":
                    return HighlightColor.Black;
            }
            return HighlightColor.NoColor;
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
        /// Gets the Sub or Super script format.
        /// </summary>
        /// <param name="baseline"></param>
        /// <returns></returns>
        private static BaselineAlignment GetBaseLine(SubSuperScript baselineAlignment)
        {
            BaselineAlignment subsuper = BaselineAlignment.Normal;
            switch (baselineAlignment)
            {
                case SubSuperScript.None:
                    subsuper = BaselineAlignment.Normal;
                    break;
                case SubSuperScript.SubScript:
                    subsuper = BaselineAlignment.Subscript;
                    break;
                case SubSuperScript.SuperScript:
                    subsuper = BaselineAlignment.Superscript;
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
                bitmap = new BitmapImage();
#if WPF
                bitmap.BeginInit();
                bitmap.SetSource(ms);
                bitmap.EndInit();
#else
                bitmap.SetSource(ms);
#endif
                return bitmap;
            }
            return null;
        }
        /// <summary>
        /// Line space converter.
        /// </summary>
        /// <param name="lineSpace"></param>
        /// <returns></returns>
        internal static void ParseLineSpace(WParagraphFormat srcFormat, ParagraphFormat destFormat)
        {
            if (srcFormat.LineSpacing < 0)
            {
                destFormat.LineSpacing = -(srcFormat.LineSpacing * 96) / 72;
                destFormat.LineSpacingType = LineSpacingType.Exactly;
            }
            else
            {
                switch (srcFormat.LineSpacingRule)
                {
                    case DocIO.LineSpacingRule.AtLeast:
                        destFormat.LineSpacingType = LineSpacingType.AtLeast;
                        destFormat.LineSpacing = (srcFormat.LineSpacing * 96) / 72;
                        break;
                    case DocIO.LineSpacingRule.Exactly:
                        destFormat.LineSpacingType = LineSpacingType.Exactly;
                        destFormat.LineSpacing = (srcFormat.LineSpacing * 96) / 72;
                        break;
                    default:
                        destFormat.LineSpacingType = LineSpacingType.Multiple;
                        destFormat.LineSpacing = srcFormat.LineSpacing / 12f;
                        break;
                }
            }
        }
        #endregion
    }
}
