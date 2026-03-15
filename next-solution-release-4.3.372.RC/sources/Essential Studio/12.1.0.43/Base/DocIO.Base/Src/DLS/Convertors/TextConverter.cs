#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion


#region file using directives
using System;
using System.IO;
using Syncfusion.DocIO.DLS;
using System.Collections.Specialized;
using System.Collections;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents TextConverter.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class TextConverter
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private StreamWriter m_writer;
        private string m_text = "";
        /// <summary>
        /// 
        /// </summary>
        private int m_curSectionIndex = 0;
        private bool m_bGetString = false;
        private WordDocument m_document;
        private WParagraph m_lastPara;
        private bool isFieldEnd = false;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="TextConverter"/> class.
        /// </summary>
        public TextConverter()
        {
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public string GetText(WordDocument document)
        {
            m_document = document;
            m_bGetString = true;
            Write();
            m_bGetString = false;

            return m_text;
        }
        /// <summary>
        /// By means of StreamWriter writes WordDocument to TXT format.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="document">The document.</param>
        public void Write(StreamWriter writer, IWordDocument document)
        {
            m_writer = writer;
            m_document = document as WordDocument;
            Write();
        }
        /// <summary>
        /// Reads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="document">The document.</param>
        public void Read(StreamReader reader, IWordDocument document)
        {
            string text = reader.ReadToEnd();
            Read(text, document);
        }
        /// <summary>
        /// Reads the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="document">The document.</param>
        internal void Read(string text, IWordDocument document)
        {
            text = text.Replace("\r\n", "\n");
            text = text.Replace("\r", "\n");
            string[] textlines = text.Split("\n".ToCharArray());

            if (document.LastParagraph == null)
            {
                if (document.LastSection == null)
                    document.EnsureMinimal();
                else
                    document.LastSection.Body.AddParagraph();
            }

            for (int i = 0, count = textlines.Length; i < count; i++)
            {
                string line = textlines[i];
                line = line.Trim("\r".ToCharArray());
                if (i > 0 && (i + 1 < count || !string.IsNullOrEmpty(line)))
                    document.LastSection.Body.AddParagraph();
                if (!string.IsNullOrEmpty(line))
                    (document.LastParagraph as IWParagraph).AppendText(line);
            }

            InitBuiltinDocumentProperties(text, textlines, document);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textLines"></param>
        private void InitBuiltinDocumentProperties(string text, string[] textLines, IWordDocument doc)
        {
            int paraCount = textLines.Length;
            int wordCount = 0;

            foreach (string line in textLines)
            {
                if (line == "\r" || line == string.Empty)
                {
                    paraCount--;
                }
                else 
                {
                    string[] words = line.Split(" ".ToCharArray());
                    foreach(string word in words)
                    {
                        if(word != string.Empty)
                            wordCount++;
                    }
                }
            }

            text = text.Replace(" ", string.Empty);
            text = text.Replace("\n", string.Empty);
            text = text.Replace("\r", string.Empty);
#if !SILVERLIGHT && !WP
            doc.BuiltinDocumentProperties.ParagraphCount = paraCount;
            doc.BuiltinDocumentProperties.WordCount = wordCount;
            doc.BuiltinDocumentProperties.CharCount = text.Length;
#endif
        }
        /// <summary>
        /// Writes document header/footer body.
        /// </summary>
        /// <param name="document"></param>
        protected void WriteHFBody(WordDocument document)
        {
            //      int secCount = m_wordDoc.Sections.Count;
            //      
            //      for( int i = 0; i < secCount; i++ )
            //      {
            //        
            //        m_writer.WriteLine( "\r\n< " + i + " Section Headers Footers>" );
            //        for( int j = 0; j < 6; j++ )
            //        {
            //          IParagraphCollection collection = m_wordDoc.Sections[i].HeadersFooters[ j ];
            //        
            //          if( collection.Count > 0)
            //          {
            //            m_writer.WriteLine( "<" + (HeaderType)j + ">" );
            //            WriteParagraphs( collection, false );
            //            m_writer.WriteLine( "</" + (HeaderType)j + ">" );
            //          }
            //        }
            //        m_writer.WriteLine( "</ " + i + " Section Headers Footers>" );
            //      }
        }
        /// <summary>
        /// Writes document body.
        /// </summary>
        /// <param name="body">The body.</param>
        protected void WriteBody(ITextBody body)
        {
            int bodyItemsCount = body.ChildEntities.Count - 1;
            TextBodyItem bodyItem = null;

            for (int i = 0; i <= bodyItemsCount; i++)
            {
                bodyItem = body.ChildEntities[i] as TextBodyItem;

                switch (bodyItem.EntityType)
                {
                    case EntityType.Paragraph:
                        bool isLastPara = (bodyItem as WParagraph == m_lastPara) ? true : false;
                        WriteParagraph(bodyItem as IWParagraph, isLastPara);
                        break;
                    case EntityType.Table:
                        WriteTable(bodyItem as IWTable);
                        break;
                }
            }
        }
        /// <summary>
        /// Writes paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="lastPara">if it is last paragraph, set to <c>true</c>.</param>
        protected void WriteParagraph(IWParagraph paragraph, bool lastPara)
        {
            if (!(paragraph.ListFormat.IsEmptyList || paragraph.ChildEntities.Count == 0 || (paragraph as WParagraph).SectionEndMark))
                WriteList(paragraph);

            for (int i = 0, len = paragraph.Items.Count; i < len; i++)
            {
                IParagraphItem item = paragraph[i];
                //Check whether the isSeparatorEnd property true and skip paragraph items from writing until the field field encounted
                if (isFieldEnd && (item is WFieldMark) && (item as WFieldMark).Type == FieldMarkType.FieldEnd)
                    isFieldEnd = false;
                //Check isSeparatorEnd property true or not
                if (!isFieldEnd)
                {
                    switch (item.EntityType)
                    {
                        case EntityType.Break:
                            if ((item as Break).BreakType == BreakType.LineBreak)
                                WriteNewLine();
                            break;
                        case EntityType.MergeField:
                            WMergeField mergeField = item as WMergeField;
                            string mergeFieldText = (mergeField.FieldValue == string.Empty) ? mergeField.UpdateMergeFieldText(mergeField) : mergeField.FieldValue;
                            WriteText(mergeFieldText);
                            break;
                        case EntityType.Field:
                            WField field = item as WField;
                            switch (field.FieldType)
                            {
                                case FieldType.FieldDocVariable:
                                    string docFieldText = field.Document.Variables[field.FieldValue];
                                    WriteText(docFieldText);
                                    if (field.FieldEnd != null && field.FieldEnd.OwnerParagraph != null)
                                        isFieldEnd = true;
                                    break;
                                case FieldType.FieldUnknown:
                                    string fiedUnknownText = field.FieldCode;
                                    WriteText(fiedUnknownText);
                                    if (field.FieldEnd != null && field.FieldEnd.OwnerParagraph != null)
                                        isFieldEnd = true;
                                    break;
                            }
                            break;
                        case EntityType.TextRange:
                            WriteText((item as IWTextRange).Text);
                            break;
                        case EntityType.TextBox:
                            WriteBody((item as WTextBox).TextBoxBody);
                            break;
                        case EntityType.AutoShape:
                            WriteBody((item as Shape).TextBody);
                            break;
                    }
                }
            }

            if (!lastPara)
            {
                WriteNewLine();
            }
        }
        /// <summary>
        /// Writes table.
        /// </summary>
        /// <param name="table"></param>
        protected void WriteTable(IWTable table)
        {
            foreach (WTableRow row in table.Rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    WriteBody(cell);
                }
            }
        }
        /// <summary>
        /// Writes end of section.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="lastSection">if it is last section, set to <c>true</c>.</param>
        protected void WriteSectionEnd(IWSection section, bool lastSection)
        {
            if (m_bGetString)
            {
                m_text += "\r\n";
            }
            else if (!lastSection)
            {
                m_writer.WriteLine("");
            }
            m_curSectionIndex++;
        }
        /// <summary>
        /// Writes text
        /// </summary>
        /// <param name="text"></param>
        protected void WriteText(string text)
        {
            if (m_bGetString)
            {
                m_text += text;
            }
            else
            {
                m_writer.Write(text);
            }
        }
        /// <summary>        /// Write list.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        protected void WriteList(IWParagraph paragraph)
        {
            //Get Current list format
            WListFormat listFormat = GetCurrentListFormat(paragraph as WParagraph);
            if (listFormat != null && listFormat.CurrentListStyle != null && listFormat.ListType != ListType.NoList)
            {
                //Get Current list level
                WListLevel listLevel = GetCurrentListLevel(listFormat, paragraph as WParagraph);
                if (listLevel.PatternType == ListPatternType.Bullet)
                {
                    if (m_bGetString)
                    {
                        m_text += "* ";
                    }
                    else
                    {
                        m_writer.Write("* ");
                    }
                }
#if !SILVERLIGHT && !WP
                else
                {
                    //Updates the list value
                    string listValue = (this.m_document as WordDocument).UpdateListValue((WParagraph)paragraph, listFormat, listLevel);
                    if (m_bGetString)
                    {
                        m_text += listValue + "  ";
                    }
                    else
                    {
                        m_writer.Write(listValue + "  ");
                    }
                }
#endif
            }
        }
        /// <summary>
        /// Get Current list format of the paragraph
        /// </summary>
        /// <returns></returns>
        private WListFormat GetCurrentListFormat(WParagraph paragraph)
        {
            WListFormat listFormat = null;
            WParagraphStyle pStyle = paragraph.ParaStyle as WParagraphStyle;
            if (paragraph.ListFormat.ListType != ListType.NoList)
                listFormat = paragraph.ListFormat;
            // Get the list format from the paragraph style
            else
            {
                while (pStyle != null)
                {
                    if (pStyle.ListFormat.ListType != ListType.NoList || pStyle.ListFormat.IsEmptyList)
                    {
                        listFormat = pStyle.ListFormat;
                        break;
                    }
                    else
                        pStyle = pStyle.BaseStyle;
                }
            }
            return listFormat;
        }
        /// <summary>
        /// Get CurrentListLevel of the paragraph
        /// </summary>
        /// <returns></returns>
        private WListLevel GetCurrentListLevel(WListFormat listFormat,WParagraph paragraph)
        {
            ListStyle listStyle = listFormat.CurrentListStyle;
            int levelNumber = 0;
            WParagraphStyle pStyle = paragraph.ParaStyle as WParagraphStyle;
            if (paragraph.ListFormat.HasKey(WListFormat.ListLevelNumberKey))
                levelNumber = paragraph.ListFormat.ListLevelNumber;
            //Get the level number from the paragraph style list format
            else
            {
                while (pStyle != null)
                {
                    if (pStyle.ListFormat.HasKey(WListFormat.ListLevelNumberKey))
                    {
                        levelNumber = pStyle.ListFormat.ListLevelNumber;
                        break;
                    }
                    else
                        pStyle = pStyle.BaseStyle;
                }
            }
            // Updates current list level.
            WListLevel level = listStyle.GetNearLevel(levelNumber);

            ListOverrideStyle listOverrideStyle = null;
            if (listFormat.LFOStyleName != null
                && listFormat.LFOStyleName.Length > 0)
                listOverrideStyle = (m_document as WordDocument).ListOverrides.FindByName(listFormat.LFOStyleName);
            if (listOverrideStyle != null
                && listOverrideStyle.OverrideLevels.HasOverrideLevel(levelNumber)
                && listOverrideStyle.OverrideLevels[levelNumber].OverrideFormatting)
                level = listOverrideStyle.OverrideLevels[levelNumber].OverrideListLevel;
            return level;
        }
        /// <summary>
        /// Writes new line.
        /// </summary>
        protected void WriteNewLine()
        {
            if (m_bGetString)
            {
                m_text += "\r\n";
            }
            else
            {
                m_writer.WriteLine("");
            }
        }
        /// <summary>
        /// Updates the last paragraph.
        /// </summary>
        /// <param name="document">The document.</param>
        private void UpdateLastPara()
        {
            if (m_document.LastSection.HeadersFooters.Footer.ChildEntities.Count > 0)
                m_lastPara = m_document.LastSection.HeadersFooters.Footer.LastParagraph as WParagraph;
            else
                m_lastPara = m_document.LastParagraph;
        }
        /// <summary>
        /// Writes the content of the document to the text file.
        /// </summary>
        private void Write()
        {
            WSection section = null;
            int sectionCount = m_document.Sections.Count - 1;
            bool isLastSection = false;
            UpdateLastPara();

            for (int i = 0; i <= sectionCount; i++)
            {
                section = m_document.Sections[i] as WSection;
                isLastSection = (i == sectionCount) ? true : false;

                WriteBody(GetHeader(section, m_curSectionIndex));
                WriteBody(section.Body);
                WriteSectionEnd(section, isLastSection);
                WriteBody(GetFooter(section, m_curSectionIndex - 1));
            }
#if !SILVERLIGHT && !WP
            // Clears the list collection.
            (m_document as WordDocument).ClearLists();
#endif
        }
        /// <summary>
        /// Returns the footers referred by the current section
        /// </summary>
        /// <param name="section">Current section</param>
        /// <param name="sectionIndex">Current section index</param>
        /// <returns>returns the footer referred by the current section by comparing the current and preceeding section</returns>
        private ITextBody GetFooter(WSection section, int sectionIndex)
        {
            HeaderFooterType currFooterType = section.PageSetup.DifferentFirstPage ? HeaderFooterType.FirstPageFooter : HeaderFooterType.OddFooter;
            if (section.HeadersFooters[currFooterType].LinkToPrevious && sectionIndex > 0)
            {
                int currIndex = sectionIndex - 1;
                while (currIndex >= 0)
                {
                    WSection preceedingSection = m_document.Sections[currIndex];

                    HeaderFooter footer = preceedingSection.HeadersFooters[currFooterType];
                    
                    if (currFooterType == footer.Type && footer.LinkToPrevious)
                    {
                        currIndex--;
                        continue;
                    }
                    else if (currFooterType == footer.Type && !footer.LinkToPrevious)
                    {
                        section.HeadersFooters[currFooterType] = preceedingSection.HeadersFooters[currFooterType];
                        break;
                    }
                }
            }
            if (section.PageSetup.DifferentFirstPage)
                return section.HeadersFooters.FirstPageFooter;
            else
                return section.HeadersFooters.Footer;
        }
        /// <summary>
        /// Returns the header referred by the current section
        /// </summary>
        /// <param name="section">Current section</param>
        /// <param name="sectionIndex">Current section index</param>
        /// <returns>returns the header referred by the current section by comparing the current and preceeding section</returns>
        private ITextBody GetHeader(WSection section, int sectionIndex)
        {
            HeaderFooterType currHeaderType = section.PageSetup.DifferentFirstPage ? HeaderFooterType.FirstPageHeader : HeaderFooterType.OddHeader;
            if (section.HeadersFooters[currHeaderType].LinkToPrevious && sectionIndex > 0)
            {
                int currIndex = sectionIndex - 1;
                while (currIndex >= 0)
                {
                    WSection preceedingSection = m_document.Sections[currIndex];
                    HeaderFooter header = preceedingSection.HeadersFooters[currHeaderType];
                    if (currHeaderType == header.Type && header.LinkToPrevious)
                    {
                        currIndex--;
                        continue;
                    }
                    else if (currHeaderType == header.Type && !header.LinkToPrevious)
                    {
                        section.HeadersFooters[currHeaderType] = preceedingSection.HeadersFooters[currHeaderType];
                        break;
                    }

                }
            }
            if (section.PageSetup.DifferentFirstPage)
                return section.HeadersFooters.FirstPageHeader;
            else
                return section.HeadersFooters.Header;
        }
        #endregion
    }

    internal class Utf8Checker
    {
        /// <summary>
        /// Check if stream is utf8 encoded.
        /// Notice: stream is read completely in memory!
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>True if the whole stream is utf8 encoded.</returns>
        internal static bool IsUtf8(Stream stream)
        {
            byte[] bomBytes = new byte[stream.Length > 4 ? 4 : stream.Length];
            stream.Position = 0;
            stream.Read(bomBytes, 0, bomBytes.Length);
            stream.Position = 0;
            if (bomBytes.Length >= 2 && (bomBytes[0] == 0xff && bomBytes[1] == 0xfe //UTF-16 (LE) BOM
                || bomBytes[0] == 0xfe && bomBytes[1] == 0xff)) //UTF-16 (BE) BOM
                return false;
            if (bomBytes.Length >= 3)
            {
                if (bomBytes[0] == 0xef && bomBytes[1] == 0xbb && bomBytes[2] == 0xbf) //UTF-8 BOM
                    return true;
                if (bomBytes[0] == 0x2b && bomBytes[1] == 0x2f && bomBytes[2] == 0x76) //UTF-7 BOM
                    return false;
            }
            if (bomBytes.Length == 4 && (bomBytes[0] == 0xff && bomBytes[1] == 0xfe && bomBytes[2] == 0 && bomBytes[3] == 0 //UTF-32 (LE) BOM
                || bomBytes[0] == 0 && bomBytes[1] == 0 && bomBytes[2] == 0xfe && bomBytes[3] == 0xff)) //UTF-32 (BE) BOM
                return false;
            bool isUtf8Stream = !HasExtendedASCIICharacter(stream);
            stream.Position = 0;
            return isUtf8Stream;
        }
        /// <summary>
        /// Determines whether the specified stream has extended ASCII character.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>
        ///   <c>true</c> if the specified stream has extended ASCII character; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasExtendedASCIICharacter(Stream stream)
        {
            stream.Position = 0;
            while (stream.Position < stream.Length)
            {
                int startByte = stream.ReadByte();
                if (startByte == -1)
                    break;
                //For 1 byte (7 bits) character, UTF8 representation is 0vvvvvvv.
                if (startByte <= 0x7F)
                    continue;
                //For 2 byte (11 bits) character, UTF8 representation is 110vvvvv 10vvvvvv.
                if (startByte >= 0xC0 && startByte <= 0xDF)
                {
                    int nextByte = stream.ReadByte();
                    if (nextByte < 0x80 || nextByte > 0xBF)
                        return true;
                    continue;
                }
                //For 3 byte (16 bits) character, UTF8 representation is 1110vvvv 10vvvvvv 10vvvvvv.
                if (startByte >= 0xE0 && startByte <= 0xEF)
                {
                    int i = 0;
                    while (i < 2)
                    {
                        int nextByte = stream.ReadByte();
                        i++;
                        if (nextByte < 0x80 || nextByte > 0xBF)
                            return true;
                    }
                    continue;
                }
                //For 4 byte (21 bits) character, UTF8 representation is 11110vvv 10vvvvvv 10vvvvvv 10vvvvvv.
                if (startByte >= 0xF0 && startByte <= 0xF7)
                {
                    int i = 0;
                    while (i < 3)
                    {
                        int nextByte = stream.ReadByte();
                        i++;
                        if (nextByte < 0x80 || nextByte > 0xBF)
                            return true;
                    }
                    continue;
                }
                return true;
            }
            return false;
        }
    }
}
