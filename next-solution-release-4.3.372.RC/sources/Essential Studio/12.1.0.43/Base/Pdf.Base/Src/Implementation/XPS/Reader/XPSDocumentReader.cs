#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Syncfusion.Compression.Zip;
using Syncfusion.XPS;
using Syncfusion.Pdf.Graphics;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Text;


namespace Syncfusion.XPS
{
    /// <summary>
    /// Represents the XPS Document reader
    /// </summary>
    internal class XPSDocumentReader : IDisposable
    {
        #region Fields
        private ZipArchive xpsFile;
        private FixedDocumentSequence documentSequence;
        private List<FixedDocument> documents;
        private List<FixedPage> pages;
        private Dictionary<string, PdfTrueTypeFont> m_fonts;

        private Regex FixedDocumentSequence = new Regex("fdseq");
        private int fPageCount = -1;
        private bool isfPageMoreThanOnce = false;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the new instance of the XPS Document reader
        /// </summary>
        /// <param name="fileName">Path to the XPS document</param>
        public XPSDocumentReader(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("fileName");

            xpsFile = new ZipArchive();
            xpsFile.Open(fileName);
        }

        /// <summary>
        /// Initializes a new instance of the XPS document reader
        /// </summary>
        /// <param name="stream">Stream containing the XPS document content</param>
        public XPSDocumentReader(Stream stream)
        {
            if (!stream.CanRead && !stream.CanSeek)
                throw new ArgumentException("Unable to open the specified document stream");

            xpsFile = new ZipArchive();
            xpsFile.Open(stream, true);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the FixedDocumentSequence
        /// </summary>
        public FixedDocumentSequence DocumentSequence
        {
            get
            {
                return documentSequence;
            }
        }

        /// <summary>
        /// Gets the Fixed documents
        /// </summary>
        public FixedDocument[] Documents
        {
            get
            {
                if (documents == null)
                    throw new ArgumentOutOfRangeException("FixedDocuments");

                return documents.ToArray();
            }
        }

        /// <summary>
        /// Gets the fixed pages
        /// </summary>
        public FixedPage[] Pages
        {
            get
            {
                if (pages == null)
                    throw new ArgumentOutOfRangeException("FixedPages");

                return pages.ToArray();
            }
        }

        /// <summary>
        /// Gets the font names
        /// </summary>
        public Dictionary<string, PdfTrueTypeFont> Fonts
        {
            get
            {
                if (m_fonts == null)
                    m_fonts = new Dictionary<string, PdfTrueTypeFont>();

                return m_fonts;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Reads the FixedDocumentSequence
        /// </summary>
        public void Read()
        {
            ReadFixedDocumentSequence();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Reads the FixedDocumentSequence
        /// </summary>
        private void ReadFixedDocumentSequence()
        {

            documentSequence = (FixedDocumentSequence)ReadElement(FixedDocumentSequence, typeof(FixedDocumentSequence));

            ReadFixedDocuments();
        }

        /// <summary>
        /// Reads the fixed documents
        /// </summary>
        private void ReadFixedDocuments()
        {
            documents = new List<FixedDocument>();

            foreach (DocumentReference reference in documentSequence.DocumentReference)
            {
                documents.Add(
                    (FixedDocument)ReadElement
                    (GetSafeName(reference.Source), typeof(FixedDocument)));
            }

            ReadFixedPages();
        }

        /// <summary>
        /// Reads the image
        /// </summary>
        /// <param name="elementName">Image element</param>
        /// <returns>Image stream</returns>
        public Stream ReadImage(string elementName)
        {
            Stream image = ReadElement(elementName);

            return image;
        }

        /// <summary>
        /// Reads the fixed pages.
        /// </summary>
        private void ReadFixedPages()
        {
            pages = new List<FixedPage>();


            foreach (FixedDocument document in documents)
            {
                foreach (PageContent page in document.PageContent)
                {
                    pages.Add(
                        (FixedPage)ReadElement(
                        GetSafeName(page.Source), typeof(FixedPage)));
                }
            }
        }

        /// <summary>
        /// Reads the elements from the fixed page.
        /// </summary>
        /// <param name="pattern">pattern of the element</param>
        /// <param name="elementType">XPSElement type</param>
        /// <returns>XPS Element</returns>
        private object ReadElement(Regex pattern, Type elementType)
        {
            int index = 0;
            if (pattern.ToString() == "fdseq")
                index = xpsFile.Find(elementType.Name + ".fdseq");
            if (index == -1)
                index = xpsFile.Find(pattern);
            object element = null;

            if (index == -1)
                throw new ArgumentException("Element not found : " + pattern.ToString());

            using (ZipArchiveItem item = xpsFile.Items[index])
            {
                if (System.IO.Path.GetExtension(item.ItemName) == ".piece")
                {
                    pattern = new Regex(pattern.ToString() + "/");
                    index = xpsFile.Find(pattern);

                    if (index == -1)
                        throw new ArgumentException("Element not found : " + pattern.ToString());

                    string fileContent = string.Empty;

                    while(index != -1)
                    {
                        using (ZipArchiveItem currentItem = xpsFile.Items[index])
                        {
                            currentItem.DataStream.Position = 0;

                            StreamReader reader = new StreamReader(currentItem.DataStream);
                            string temp = reader.ReadToEnd();
                            fileContent += (!string.IsNullOrEmpty(temp)) ? temp : "";
                            reader.Dispose();

                            int junkIndex = fileContent.LastIndexOf('>') + 1;
                            if (junkIndex < fileContent.Length)
                                fileContent = fileContent.Substring(0, junkIndex);
                        }
                        index = Find(pattern, index + 1);
                    }

                    byte[] fileBytes = System.Text.Encoding.Default.GetBytes(fileContent);
                    MemoryStream ms = new MemoryStream(fileBytes);
                    
                    XmlSerializer serializer = new XmlSerializer(elementType);

                    element = serializer.Deserialize(ms);

                    return element;
                }
                item.DataStream.Position = 0;

                using (TextReader reader = new StreamReader(item.DataStream))
                {
                    XmlSerializer serializer = new XmlSerializer(elementType);

                    element = serializer.Deserialize(reader);
                }
            }

            return element;
        }

        /// <summary>
        /// Reads the XPS element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="elementType">Element type</param>
        /// <returns>XPS element</returns>
        private object ReadElement(string elementName, Type elementType)
        {
            int index = Find(elementName);
            if (elementName.Contains("1.fpage"))
            {
                for (int x = Find(elementName) + 1; x < xpsFile.Items.Length; x++)
                {
                    if (xpsFile.Items[x].ItemName != null && xpsFile.Items[x].ItemName.Contains("1.fpage"))
                    {
                        isfPageMoreThanOnce = true;
                        break;
                    }
                }
            }
            object element = null;

            if (index == -1)
                throw new ArgumentException("Element not found : " + elementName);

            ZipArchiveItem item = xpsFile.Items[index];

            if (System.IO.Path.GetExtension(item.ItemName) == ".piece")
            {
                string fileContent = string.Empty;

                while (index != -1)
                {
                    using (ZipArchiveItem pieceItem = xpsFile[index])
                    {
                        pieceItem.DataStream.Position = 0;

                        StreamReader pieceReader = new StreamReader(pieceItem.DataStream);
                        fileContent += pieceReader.ReadToEnd();
                        pieceReader.Dispose();

                        int junkIndex = fileContent.LastIndexOf('>') + 1;
                        if (junkIndex < fileContent.Length)
                            fileContent = fileContent.Substring(0, junkIndex);

                        index = Find(elementName, index + 1);
                    }
                }

                byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(fileContent);
                MemoryStream ms = new MemoryStream(fileBytes);

                XmlSerializer serializer1 = new XmlSerializer(elementType);

                element = serializer1.Deserialize(ms);

                return element;
            }
            
            item.DataStream.Position = 0;

            TextReader reader = new StreamReader(item.DataStream);
            XmlSerializer serializer = new XmlSerializer(elementType);

            element = serializer.Deserialize(reader);
            if (element is FixedDocument && isfPageMoreThanOnce)
            {
                FixedDocument fixedDoc = element as FixedDocument;
                for (int i = 0; i < fixedDoc.PageContent.Length; i++)
                    fixedDoc.PageContent[i].Source = elementName.Replace("FixedDocument.fdoc", "") + fixedDoc.PageContent[i].Source;
            }
            return element;
        }

        /// <summary>
        /// Reads the XPS element
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <returns>XPS element</returns>
        private Stream ReadElement(string elementName)
        {
            int index = Find(elementName);
            object element = null;

            if (index == -1)
                throw new ArgumentException("Element not found : " + elementName);

            ZipArchiveItem item = xpsFile.Items[index];
            return item.DataStream;
        }

        /// <summary>
        /// Gets the safe name
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <returns>Safe name</returns>
        private string GetSafeName(string elementName)
        {
            return elementName.TrimStart(
                new char[] { '/','.'});
        }

        /// <summary>
        /// Search for a XPS element
        /// </summary>
        /// <param name="elementName"> Name of the element</param>
        /// <returns>Index of the element</returns>
        public int Find(string elementName)
        {
            return Find(elementName, 0);
        }

        /// <summary>
        /// Search for a XPS element.
        /// </summary>
        /// <param name="elementName">Name of the element</param>
        /// <param name="startIndex">Index to start search</param>
        /// <returns>Index of the element</returns>
        internal int Find(string elementName, int startIndex)
        {
            if (string.IsNullOrEmpty(elementName))
                return -1;

            int itemIndex = xpsFile.Find(elementName);

            if (itemIndex == -1)
            {
                elementName = GetSafeName(elementName);

                for (int i = startIndex; i < xpsFile.Items.Length; i++)
                {
                    if (xpsFile.Items[i].ItemName != null &&
                        xpsFile.Items[i].ItemName.IndexOf(elementName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        itemIndex = i;
                        break;
                    }
                }
            }

            return itemIndex;
        }

        /// <summary>
        /// Search for a XPS element.
        /// </summary>
        /// <param name="element">Regex of element</param>
        /// <param name="startIndex">Index to start search</param>
        /// <returns>Index of the element</returns>
        internal int Find(Regex element, int startIndex)
        {
            if (element == null)
                return -1;

            for (int i = startIndex; i < xpsFile.Items.Length; i++)
            {
                if (element.IsMatch(xpsFile.Items[i].ItemName))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Reads the font with the URI
        /// </summary>
        /// <param name="fontUri">Font URI</param>
        /// <returns>Font Name</returns>
        public Stream ReadFont(string fontUri)
        {
            MemoryStream fontStream = new MemoryStream();
            int index = Find(fontUri);
            string[] fontFormats = fontUri.Split('.');
            string fontFormat = fontFormats[fontFormats.Length - 1];

            if (fontFormat.ToLower() == "odttf")
            {
                int startIndex = fontUri.LastIndexOf('/') + 1;
                int length = fontUri.LastIndexOf('.') - startIndex;
                string fontGuid = fontUri.Substring(startIndex, length);
                fontGuid = new Guid(fontGuid).ToString("N");

                //De-Obfuscate font.
                Stream dataStream = xpsFile.Items[index].DataStream;
                dataStream.Position = 0;
                DeObfuscateFont(dataStream, fontStream, fontGuid);
                return fontStream;
            }
            else
            {
                Stream dataStream = xpsFile.Items[index].DataStream;
                dataStream.Position = 0;
                int length = (int)dataStream.Length;
                byte[] font = new byte[length];
                dataStream.Read(font, 0, length);
                return new MemoryStream(font);
            }
        }

        /// <summary>
        /// Gets the Font style
        /// </summary>
        /// <param name="fontUri">Font URI</param>
        /// <returns>Font style</returns>
        public FontStyle GetDeviceFontStyle(string fontUri)
        {
            FontStyle fontStyle = FontStyle.Regular;

            MemoryStream fontStream = new MemoryStream();
            int index = Find(fontUri);
            string fontName;
            //string fontStyle;

            int startIndex = fontUri.LastIndexOf('/') + 1;
            int length = fontUri.LastIndexOf('.') - startIndex;
            string fontGuid = fontUri.Substring(startIndex, length);
            fontGuid = new Guid(fontGuid).ToString("N");

            //De-Obfuscate font.
            Stream dataStream = xpsFile.Items[index].DataStream;
            dataStream.Position = 0;
            DeObfuscateFont(dataStream, fontStream, fontGuid);

            //Retrieve font Style.
            using (BinaryReader reader = new BinaryReader(fontStream))
            {
                TtfReader fontReader = new TtfReader(reader);

                if (fontReader.Metrics.IsBold && fontReader.Metrics.IsItalic)
                    fontStyle = FontStyle.Bold | FontStyle.Italic;

                else if (fontReader.Metrics.IsBold)
                    fontStyle = FontStyle.Bold;

                else if (fontReader.Metrics.IsItalic)
                    fontStyle = FontStyle.Italic;

            }

            return fontStyle;
        }

        /// <summary>
        /// Extract the font
        /// </summary>
        /// <param name="font">Font Stream</param>
        /// <param name="outStream">Created font</param>
        /// <param name="fontGuid">Font Guid</param>
        private void DeObfuscateFont(Stream font, Stream outStream, string fontGuid)
        {
            //..
            //..Perform an XOR operation on the first 32 bytes of the binary data of the obfuscated font 
            //..part with the array consisting of the bytes referred to by the placeholders B 37 , B 36 , 
            //..B 35 , B 34 , B 33 , B 32 , B 31 , B 30 , B 20 , B 21 , B 10 , B 11 , B 00 , B 01 , B 02 , 
            //..and B 03 , in that order and repeating the array once
            //...

            byte[] buffer = new byte[16];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Convert.ToByte(fontGuid.Substring(i * 2, 2), 16);
            }

            byte[] buffer2 = new byte[32];
            font.Read(buffer2, 0, 32);

            for (int i = 0; i < 32; i++)
            {
                int index = (buffer.Length - (i % buffer.Length)) - 1;
                buffer2[i] = (byte)(buffer2[i] ^ buffer[index]);
            }

            outStream.Write(buffer2, 0, 32);
            buffer2 = new byte[4096];

            int num = 0;
            while ((num = font.Read(buffer2, 0, 4096)) > 0)
            {
                outStream.Write(buffer2, 0, num);
            }
        }

        /// <summary>
        /// Reads the resources in the XPS document
        /// </summary>
        /// <param name="elementName">resource name</param>
        /// <returns>Resource</returns>
        public Stream ReadResource(string elementName)
        {
            int index = Find(elementName);
            if (index == -1)
                throw new Exception("Specified resource not found.");

            Stream resourceStream = xpsFile.Items[index].DataStream;
            resourceStream.Position = 0;

            return resourceStream;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose the document
        /// </summary>
        public void Dispose()
        {
            xpsFile.Dispose();
            documentSequence = null;

            if (documents != null)
                documents.Clear();

            if (pages != null)
                pages.Clear();
        }
        #endregion
    }
}
