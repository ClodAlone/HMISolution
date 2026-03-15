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
using System.Windows.Input;
using System.Text;
using System.Linq;
using System.IO;

#if WPF
namespace Syncfusion.Windows.Tools.RichTextBoxAdv
#else
namespace Syncfusion.UI.Xaml.RichTextBoxAdv
#endif
{
    internal class TextExporting
    {
        #region Constructors

        public TextExporting()
        {

        }

        #endregion

        #region Implementation

        /// <summary>
        /// Returns the Text string from the DocumentAdv and Stream
        /// </summary>
        /// <param name="textContent"></param>
        /// <param name="textStream"></param>
        /// <returns></returns>
        public static string ConvertToText(DocumentAdv textContent, Stream textStream)
        {
            return SaveAsText(textContent, textStream);
        }

        /// <summary>
        /// Returns the Text string from the DocumentAdv
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static string ConvertToText(DocumentAdv document)
        {
            return SaveAsText(document);
        }


        /// <summary>
        /// Processing the DocumentAdv 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        internal static StringBuilder TextProcessor(DocumentAdv document)
        {
            StringBuilder textProcessing = new StringBuilder();
            foreach (SectionAdv section in document.Sections)
            {
                foreach (BlockAdv block in section.Blocks)
                {
                    if (block is ParagraphAdv)
                        GetParagraphText(block as ParagraphAdv, textProcessing);
                    else if (block is TableAdv)
                        GetTableText(block as TableAdv, textProcessing);
                    if (section.Blocks.Last() != block)
                        textProcessing.AppendLine();
                }
            }
            return textProcessing;
        }
        /// <summary>
        /// Returns the Text string from the ParagraphAdv
        /// </summary>
        /// <param name="document"></param>
        private static void GetParagraphText(ParagraphAdv paragraph, StringBuilder textProcessing)
        {
            foreach (Inline inlineContent in paragraph.Inlines)
            {
                if (inlineContent is SpanAdv)
                {
                    SpanAdv spanContent = inlineContent as SpanAdv;
                    textProcessing.Append(spanContent.Text);
                }
                else if (inlineContent is FieldBeginAdv)
                {
                    FieldBeginAdv hyperLinkData = inlineContent as FieldBeginAdv;
                    //textProcessing.Append(hyperLinkData.Text);
                }
            }
        }
        /// <summary>
        /// Returns the Text string from the TableAdv
        /// </summary>
        /// <param name="document"></param>
        private static void GetTableText(TableAdv table, StringBuilder textProcessing)
        {
            foreach (TableRowAdv row in table.Rows)
            {
                foreach (TableCellAdv cell in row.Cells)
                {
                    foreach (BlockAdv block in cell.Blocks)
                    {
                        if (block is ParagraphAdv)
                            GetParagraphText(block as ParagraphAdv, textProcessing);
                        else if (block is TableAdv)
                            GetTableText(block as TableAdv, textProcessing);
                    }
                    //Appends tab after each cell, except the last cell of row.
                    if (row.Cells.Last() != cell)
                        textProcessing.Append("\t");
                }
                //Appends line break (carriage return character).
                textProcessing.AppendLine();
            }
        }

        /// <summary>
        /// Save as Text based upon the documentadv and stream
        /// </summary>
        /// <param name="documentcontent"></param>
        /// <param name="textStream"></param>
        /// <returns></returns>
        internal static string SaveAsText(DocumentAdv documentcontent, Stream textStream)
        {
            StringBuilder textBuilder = new StringBuilder();
            textBuilder = TextProcessor(documentcontent);

            //Writes the content to stream.
            StreamWriter writer = new StreamWriter(textStream);
            writer.Write(textBuilder.ToString());
            writer.Flush();
            return textBuilder.ToString();
        }

        /// <summary>
        /// Save as Text based upon the DocumentAdv
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        internal static string SaveAsText(DocumentAdv document)
        {
            StringWriter writer = new StringWriter();
            StringBuilder textBuilder = new StringBuilder();
            textBuilder = TextProcessor(document);
            writer.WriteLine(textBuilder);
            writer.Dispose();
            return textBuilder.ToString();
        }

        #endregion
    }
}
