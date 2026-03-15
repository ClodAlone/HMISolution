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
using System.IO;

namespace Syncfusion.Windows.Tools.Controls
{
    public class TextImporting
    {
        #region Constructor
        /// <summary>
        /// Create instance for the TextImporting.
        /// </summary>
        public TextImporting()
        {

        }
        #endregion

        #region Implementation

        /// <summary>
        /// Returns the DocumentAdv from the Text input.
        /// </summary>
        /// <param name="textStream"></param>
        /// <returns></returns>
        public DocumentAdv ConvertToDocumentAdv(Stream textStream)
        {
            DocumentAdv textDocument = new DocumentAdv();
            SectionAdv textSection = new SectionAdv();
            using (StreamReader reader = new StreamReader(textStream))
            {
                string readText = reader.ReadToEnd();

                string[] splitted = readText.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                for (int s = 0; s < splitted.Length; s++)
                {
                    string str = splitted[s];
                    ParagraphAdv textParagraph = new ParagraphAdv();
                    if (!string.IsNullOrEmpty(str))
                    {
                        SpanAdv spanText = new SpanAdv();
                        spanText.Text = str;
                        textParagraph.Inlines.Add(spanText);
                    }
                    textSection.Blocks.Add(textParagraph);
                }
            }
            textDocument.Sections.Add(textSection);
            textStream.Close();
            return textDocument;
        }

        public DocumentAdv ConvertToDocumentAdv(string textstring)
        {
            DocumentAdv textDocument = new DocumentAdv();
            SectionAdv textSection = new SectionAdv();

            string[] splitted = textstring.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            for (int s = 0; s < splitted.Length; s++)
            {
                string str = splitted[s];
                ParagraphAdv textParagraph = new ParagraphAdv();
                if (!string.IsNullOrEmpty(str))
                {
                    SpanAdv spanText = new SpanAdv();
                    spanText.Text = str;
                    textParagraph.Inlines.Add(spanText);
                }
                textSection.Blocks.Add(textParagraph);
            }
            textDocument.Sections.Add(textSection);
            return textDocument;
        }
        #endregion
    }
}
