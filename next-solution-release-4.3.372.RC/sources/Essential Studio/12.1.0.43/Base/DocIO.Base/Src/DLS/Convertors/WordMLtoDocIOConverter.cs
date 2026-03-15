#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT

#region file using directives
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Collections.Generic;
#endregion;

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WordMLtoDocIOConverter.
    /// </summary>
    public class WordMLtoDocIOConverter
    {
        #region Class constants
        private const string DEF_WORDML_TO_DOCIO_XSLT_RESOURCES = "Syncfusion.DocIO.Resources.wordml-to-dls-converter.xslt";
        private const string DEF_BOOKMARK_START = "Word.Bookmark.Start";
        private const string DEF_BOOKMARK_END = "Word.Bookmark.End";
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private XmlDocument m_xmlWordML = new XmlDocument();
        /// <summary>
        /// 
        /// </summary>
        private BookmarkCollection m_bookmarkList = new BookmarkCollection();
        /// <summary>
        /// 
        /// </summary>
        private XmlNamespaceManager m_nsmng;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordMLtoDocIOConverter"/> class.
        /// </summary>
        public WordMLtoDocIOConverter()
        {
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Converts the specified word as ml.
        /// </summary>
        /// <param name="pathToWordML">The path to word ML.</param>
        /// <returns></returns>
        public IWordDocument Convert(string pathToWordML)
        {
            m_xmlWordML.Load(pathToWordML);
            XslTransform transform = new XslTransform();
#if SyncfusionFramework1_0
      transform.Load( GetXsltReader(), null );
#else
            transform.Load(GetXsltReader(), null, null);
#endif
            MemoryStream stream = new MemoryStream();
            CorrectXML();
#if SyncfusionFramework1_0
      transform.Transform( m_xmlWordML, null, stream );
#else
            transform.Transform(m_xmlWordML, null, stream, null);
#endif
            stream.Position = 0;

            IWordDocument doc = new WordDocument();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);
            stream = new MemoryStream((int)stream.Length);
            xmlDoc.Save(stream);
            stream.Position = 0;

            doc.Open(stream, FormatType.Xml);

            return doc;
        }

        #endregion

        #region Class helper methods
        /// <summary>
        /// Corrects the XML.
        /// </summary>
        private void CorrectXML()
        {
            InitNamespaceManager();
            XmlNode body = m_xmlWordML.DocumentElement.SelectSingleNode("w:body", m_nsmng);
            XmlNodeList sections = body.SelectNodes("wx:sect", m_nsmng);

            foreach (XmlNode section in sections)
            {
                XmlNodeList paragraphs = section.SelectNodes("w:p", m_nsmng);

                foreach (XmlNode paragraph in paragraphs)
                {
                    ModifyParagraph(paragraph);
                }
            }
        }
        /// <summary>
        /// Modifies the paragraph.
        /// </summary>
        private void ModifyParagraph(XmlNode paragraph)
        {
            XmlNodeList bokmarks = paragraph.SelectNodes("aml:annotation", m_nsmng);
            XmlNodeList pictures = paragraph.SelectNodes("w:pict", m_nsmng);

            foreach (XmlNode bookmark in bokmarks)
            {
                ModifyBookmark(bookmark);
            }

            foreach (XmlNode picture in pictures)
            {
                ModifyPicture(picture);
            }
        }
        /// <summary>
        /// Modifies the bookmark.
        /// </summary>
        /// <param name="bookmark">The bookmark.</param>
        private void ModifyBookmark(XmlNode bookmark)
        {
            string str_type = bookmark.Attributes["type", m_xmlWordML.DocumentElement.GetNamespaceOfPrefix("w")].InnerText;
            string str_name = string.Empty;

            if (bookmark.Attributes["name", m_xmlWordML.DocumentElement.GetNamespaceOfPrefix("w")] != null)
                str_name = bookmark.Attributes["name", m_xmlWordML.DocumentElement.GetNamespaceOfPrefix("w")].InnerText;

            string str_id = bookmark.Attributes["id", m_xmlWordML.DocumentElement.GetNamespaceOfPrefix("aml")].InnerText;

            switch (str_type)
            {
                case DEF_BOOKMARK_START:
                    {
                        m_bookmarkList.Add(str_name);
                        break;
                    }
                case DEF_BOOKMARK_END:
                    {
                        XmlAttribute name = m_xmlWordML.CreateAttribute("w:name", m_xmlWordML.DocumentElement.GetNamespaceOfPrefix("w"));
                        name.InnerText = m_bookmarkList[str_id];
                        bookmark.Attributes.Append(name);
                        break;
                    }
            }
        }
        /// <summary>
        /// Modifies the picture.
        /// </summary>
        /// <param name="picture">The picture.</param>
        private void ModifyPicture(XmlNode picture)
        {
            XmlNode image = picture.SelectSingleNode("w:binData", m_nsmng);
            XmlNode shape = picture.SelectSingleNode("v:shape", m_nsmng);
            Image img = null;

            if (image != null)
            {
                img = ReadImage(image, false);
            }

            double width = img.Width;
            double height = img.Height;

            if (shape != null)
            {
                XmlAttribute imgWidth = m_xmlWordML.CreateAttribute("imgWidth");
                imgWidth.InnerText = width.ToString(CultureInfo.InvariantCulture);
                shape.Attributes.Append(imgWidth);
                XmlAttribute imgHeight = m_xmlWordML.CreateAttribute("imgHeight");
                imgHeight.InnerText = height.ToString(CultureInfo.InvariantCulture);
                shape.Attributes.Append(imgHeight);
            }
        }
        /// <summary>
        /// Initialize the namespace manager.
        /// </summary>
        private void InitNamespaceManager()
        {
            m_nsmng = new XmlNamespaceManager(m_xmlWordML.NameTable);
            m_nsmng.AddNamespace("w", m_xmlWordML.DocumentElement.GetNamespaceOfPrefix("w"));
            m_nsmng.AddNamespace("wx", m_xmlWordML.DocumentElement.GetNamespaceOfPrefix("wx"));
            m_nsmng.AddNamespace("aml", m_xmlWordML.DocumentElement.GetNamespaceOfPrefix("aml"));
            m_nsmng.AddNamespace("v", m_xmlWordML.DocumentElement.GetNamespaceOfPrefix("v"));
        }
        /// <summary>
        /// Gets the XSLT reader.
        /// </summary>
        /// <returns></returns>
        private static XmlReader GetXsltReader()
        {
            Assembly execAssm = Assembly.GetExecutingAssembly();
            Stream stream = execAssm.GetManifestResourceStream(DEF_WORDML_TO_DOCIO_XSLT_RESOURCES);
            return new XmlTextReader(stream);
        }
        /// <summary>
        /// Reads the binary element.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private byte[] ReadBinaryElement(XmlNode node)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(node.OuterXml));
            reader.Read();
            int base64len = 0;
            byte[] resData = new byte[0];
            byte[] base64 = new byte[1000];

            do
            {
                base64len = reader.ReadBase64(base64, 0, base64.Length);
                byte[] newData = new byte[resData.Length + base64len];
                resData.CopyTo(newData, 0);
                Array.Copy(base64, 0, newData, resData.Length, base64len);
                resData = newData;

                if (base64len < base64.Length)
                {
                    break;
                }
                else
                {
                    base64 = new byte[resData.Length * 2];
                }
            } while (!reader.EOF);

            return resData;
        }
        /// <summary>
        /// Reads the image.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="isMetaFile">if it is meta file, set to <c>true</c>.</param>
        /// <returns></returns>
        private Image ReadImage(XmlNode node, bool isMetaFile)
        {
            byte[] buf = ReadBinaryElement(node);
            Image image = null;

            if (buf.Length > 0)
            {
                MemoryStream memStream = new MemoryStream(buf);

                if (isMetaFile)
                {
                    image = new Metafile(memStream);
                }
                else
                {
                    image = new Bitmap(memStream);
                }
            }

            return image;
        }

        #endregion

        #region Class internal declarations
        /// <summary>
        /// 
        /// </summary>
        internal class Bookmark
        {
            #region Class members
            /// <summary>
            /// 
            /// </summary>
            private string m_strName;
            /// <summary>
            /// 
            /// </summary>
            private string m_strID;
            #endregion

            #region Class properties
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name
            {
                get
                {
                    return m_strName;
                }
                set
                {
                    m_strName = value;
                }
            }
            /// <summary>
            /// Gets or sets the ID.
            /// </summary>
            /// <value>The ID.</value>
            public string ID
            {
                get
                {
                    return m_strID;
                }
                set
                {
                    m_strID = value;
                }
            }
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        internal class BookmarkCollection : List<Bookmark>
        {
            #region Class members
            /// <summary>
            /// 
            /// </summary>
            private int m_iID = 0;
            #endregion

            #region Class Public Methods
            /// <summary>
            /// Gets the <see cref="String"/> with the specified name.
            /// </summary>
            /// <value></value>
            public string this[string id]
            {
                get
                {
                    string strReturn = string.Empty;

                    foreach (Bookmark bookmark in this)
                    {
                        if (bookmark.ID == id)
                        {
                            strReturn = bookmark.Name;
                            break;
                        }
                    }

                    return strReturn;
                }
            }

            /// <summary>
            /// Adds an object to the end of the <see cref="T:System.Collections.ArrayList"/>.
            /// </summary>
            /// <param name="value">The <see cref="T:System.Object"/> to be added to the end of the <see cref="T:System.Collections.ArrayList"/>. The value can be <see langword="null"/>.</param>
            /// <returns>
            /// The <see cref="T:System.Collections.ArrayList"/> index at which the <paramref name="value"/> has
            /// been added.
            /// </returns>
            /// <exception cref="T:System.NotSupportedException">
            /// 	<para>The <see cref="T:System.Collections.ArrayList"/> is read-only.</para>
            /// 	<para>-or-</para>
            /// 	<para>The <see cref="T:System.Collections.ArrayList"/> has a fixed size.</para>
            /// </exception>
            public int Add(Bookmark bookmark)
            {
                bookmark.ID = m_iID.ToString();
                m_iID++;
                base.Add(bookmark);
                return m_iID - 1;
            }

            /// <summary>
            /// Adds the bookmark.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <returns></returns>
            public int Add(string name)
            {
                Bookmark bookmark = new Bookmark();
                bookmark.Name = name;
                bookmark.ID = m_iID.ToString();
                m_iID++;
                base.Add(bookmark);
                return m_iID - 1;
            }

            #endregion
        }
        #endregion
    }
}

#endif
