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
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for DocIOtoWordMLConverter.
    /// </summary>
    public class DocIOtoWordMLConverter
    {
        #region Class constants
        private const string DEF_DOCIO_XSLT_RESOURCES = "Syncfusion.DocIO.Resources.dls-to-wordml-converter.xslt";
        private const string DEF_SECTIONS = "sections";
        private const string DEF_PATH_FORMAT = "{0}/{1}";
        private const string DEF_SECTION = "section";
        private const string DEF_BODY = "body";
        private const string DEF_PARAGRAPHS = "paragraphs";
        private const string DEF_PARAGRAPH = "paragraph";
        private const string DEF_BUILTIN_PROPERTIES = "builtin-properties";
        private const string DEF_PAGE_SETUP = "page-setup";
        private const string DEF_COLUMNS = "columns";
        private const string DEF_HEADERS_FOOTERS = "headers-footers";
        private const string DEF_ITEMS = "items";
        private const string DEF_ITEM = "item";
        private const string DEF_IMAGE_NAME_FORMAT = "wordml://{0}_{1}.png";
        private const string DEF_BREAKCODE = "BreakCode";
        private const string DEF_BREAKCODE_VALUE_NOBREAK = "NoBreak";
        private const string DEF_BREAKCODE_VALUE_NEWPAGE = "NewPage";
        private const string DEF_ATTRIBUTE_VALUE_TRUE = "True";
        private const string DEF_TYPE_PARAMETR = "type";
        private const string DEF_ITEM_TYPE_TABLE = "Table";
        private const string DEF_ITEM_TYPE_PICTURE = "Picture";
        private const string DEF_ITEM_TYPE_BOOKMARKSTART = "BookmarkStart";
        private const string DEF_ITEM_TYPE_BOOKMARKEND = "BookmarkEnd";
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private bool m_bNewPage = false;
        /// <summary>
        /// 
        /// </summary>
        private int[] m_continue;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bBreakBefore = false;
        /// <summary>
        /// 
        /// </summary>
        private int m_imageCount = 0;
        /// <summary>
        /// 
        /// </summary>
        private BookmarkCollection m_bookmarkList = new BookmarkCollection();
        /// <summary>
        /// 
        /// </summary>
        private XmlDocument m_outXml = new XmlDocument();
        /// <summary>
        /// 
        /// </summary>
        private WordDocument m_wordDoc;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="DocIOtoWordMLConverter"/> class.
        /// </summary>
        public DocIOtoWordMLConverter()
        {
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Converts dls xml to word ML.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="pathToWordML">The path to word ML.</param>
        public void Convert(IWordDocument doc, string pathToWordML)
        {
            XslTransform transform = new XslTransform();
#if SyncfusionFramework1_0
      transform.Load( GetXsltReader(), null );    
#else
            transform.Load(GetXsltReader(), null, null);
#endif
            MemoryStream memStream = new MemoryStream();
            m_wordDoc = (WordDocument)doc;
            doc.Save(memStream, FormatType.Xml);
            memStream.Position = 0;
            m_outXml.Load(memStream);
            CorrectDlsXml();
            XmlTextWriter writer = new XmlTextWriter(pathToWordML, Encoding.UTF8);
#if SyncfusionFramework1_0
      transform.Transform( m_outXml, null, writer );
#else
            transform.Transform(m_outXml, null, writer, null);
#endif
            writer.Close();
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static XmlReader GetXsltReader()
        {
            Assembly execAssm = Assembly.GetExecutingAssembly();
            Stream stream = execAssm.GetManifestResourceStream(DEF_DOCIO_XSLT_RESOURCES);
            return new XmlTextReader(stream);
        }
        /// <summary>
        /// Corrects the defect in dls-xml.
        /// </summary>
        private void CorrectDlsXml()
        {
            ModifySections();
            ModifyBuiltinProperties();
        }
        /// <summary>
        /// Clones the sections.
        /// </summary>
        private void ModifySections()
        {
            XmlNodeList sections =
              m_outXml.DocumentElement.SelectNodes(string.Format(DEF_PATH_FORMAT, DEF_SECTIONS, DEF_SECTION));

            int i = 0;
            m_continue = new int[sections.Count];

            foreach (XmlNode section in sections)
            {
                m_continue[i] = 0;
                if (section.Attributes[DEF_BREAKCODE] != null)
                    if (section.Attributes[DEF_BREAKCODE].InnerText == DEF_BREAKCODE_VALUE_NOBREAK)
                        m_continue[i] = 1;

                i++;
            }

            i = 1;

            foreach (XmlNode section in sections)
            {
                ModifySection(section, sections.Count, i);
                i++;
            }
        }
        /// <summary>
        /// Clones the section.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="count">The count.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private void ModifySection(XmlNode node, int count, int index)
        {
            if (node.Attributes[DEF_BREAKCODE] != null)
            {
                m_bNewPage = (node.Attributes[DEF_BREAKCODE].InnerText == DEF_BREAKCODE_VALUE_NEWPAGE);
            }

            if (m_continue[index - 1] == 1)
            {
                m_bNewPage = true;
            }

            if (count == 1 || index == count)
            {
                m_bNewPage = false;
            }

            XmlAttribute propInEndPar = m_outXml.CreateAttribute("PropInEndPar");
            propInEndPar.InnerText = m_bNewPage.ToString();
            node.Attributes.Append(propInEndPar);

            XmlNode _bodyNode = node.SelectSingleNode(DEF_BODY);
            XmlNode _paragraphs = _bodyNode.SelectSingleNode(DEF_PARAGRAPHS);
            XmlNode _pagesetup = node.SelectSingleNode(DEF_PAGE_SETUP);
            XmlNode _columns = node.SelectSingleNode(DEF_COLUMNS);
            XmlNode _hdrsftrs = node.SelectSingleNode(DEF_HEADERS_FOOTERS);

            if (_paragraphs != null)
            {
                foreach (XmlNode _paragraph in _paragraphs.ChildNodes)
                {
                    ModifyParagraph(_paragraph);
                }
            }
            if (m_bNewPage)
            {
                XmlElement paragraph = m_outXml.CreateElement(DEF_ITEM);
                XmlAttribute type = m_outXml.CreateAttribute("type");
                type.InnerText = "Paragraph";
                XmlAttribute sctPrp = m_outXml.CreateAttribute("SctPrp");
                sctPrp.InnerText = DEF_ATTRIBUTE_VALUE_TRUE;
                paragraph.Attributes.Append(type);
                paragraph.Attributes.Append(sctPrp);

                if (m_continue[index - 1] == 1)
                {
                    paragraph.SetAttribute("Continue", DEF_ATTRIBUTE_VALUE_TRUE);
                }

                paragraph.AppendChild(_pagesetup.Clone());
                paragraph.AppendChild(_hdrsftrs.Clone());

                if (_columns != null)
                {
                    paragraph.AppendChild(_columns.Clone());
                    _columns.ParentNode.RemoveChild(_columns);
                }

                _paragraphs.AppendChild(paragraph);
                _pagesetup.ParentNode.RemoveChild(_pagesetup);
                _hdrsftrs.ParentNode.RemoveChild(_hdrsftrs);
            }
        }

        /// <summary>
        /// Clones the paragraph.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private void ModifyParagraph(XmlNode node)
        {
            if (node.ChildNodes.Count > 0)
            {
                XmlNode paragraph_format = node.SelectSingleNode("paragraph-format");

                if (paragraph_format != null)
                {
                    if (paragraph_format.Attributes.Count > 0)
                        if (paragraph_format.Attributes["PageBreakAfter"] != null)
                            m_bBreakBefore = (paragraph_format.Attributes["PageBreakAfter"].InnerText == DEF_ATTRIBUTE_VALUE_TRUE.ToLower());
                }
            }

            if (node.Attributes[DEF_TYPE_PARAMETR].Value == DEF_ITEM_TYPE_TABLE)
            {
                ModifyTable(node);
            }

            XmlNode _items = node.SelectSingleNode(DEF_ITEMS);

            if (_items != null)
                ModifyItems(_items);

            if (m_bBreakBefore)
            {
                XmlAttribute breakBefore = m_outXml.CreateAttribute("BreakBefore");
                breakBefore.InnerText = DEF_ATTRIBUTE_VALUE_TRUE;
                node.Attributes.Append(breakBefore);
                m_bBreakBefore = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        private void ModifyTable(XmlNode table)
        {
            TableGrid tableGrid = new TableGrid();

            tableGrid.Parce(table);
            tableGrid.Save();
        }

        /// <summary>
        /// Clones the items.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private void ModifyItems(XmlNode node)
        {
            XmlNodeList itemList = node.SelectNodes(DEF_ITEM);

            foreach (XmlNode _item in itemList)
            {
                if (_item.Attributes[DEF_TYPE_PARAMETR] != null)
                {
                    switch (_item.Attributes[DEF_TYPE_PARAMETR].InnerText)
                    {
                        case DEF_ITEM_TYPE_PICTURE:
                            {
                                ModifyPicture(_item);
                                break;
                            }
                        case DEF_ITEM_TYPE_BOOKMARKSTART:
                            {
                                int id = m_bookmarkList.Add(_item.Attributes["BookmarkName"].InnerText);
                                XmlAttribute bookmarkID = m_outXml.CreateAttribute("bookmarkID");
                                bookmarkID.InnerText = id.ToString();
                                _item.Attributes.Append(bookmarkID);
                                break;
                            }
                        case DEF_ITEM_TYPE_BOOKMARKEND:
                            {
                                string id = m_bookmarkList[_item.Attributes["BookmarkName"].InnerText];
                                XmlAttribute bookmarkID = m_outXml.CreateAttribute("bookmarkID");
                                bookmarkID.InnerText = id.ToString();
                                _item.Attributes.Append(bookmarkID);
                                break;
                            }
                        case "Break":
                            {
                                m_bBreakBefore = true;
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Clones the built-in properties.
        /// </summary>
        private void ModifyBuiltinProperties()
        {
            XmlNode builtin_properties = m_outXml.DocumentElement.SelectSingleNode(DEF_BUILTIN_PROPERTIES);

            if (builtin_properties.Attributes["EditTime"] != null)
            {
                //XmlAttribute attribute = builtin_properties.Attributes[ "EditTime" ];
                //string sourse =
                //  attribute.InnerText.Substring( attribute.InnerText.IndexOf( '+' ),
                //  attribute.InnerText.Length - attribute.InnerText.IndexOf( '+' ) );
                //DateTime editing = DateTime.Parse( attribute.InnerText );
                //DateTime corectedTime = DateTime.Parse( sourse.Remove( 0, 1 ) );
                //int minutes = ( editing.Hour - corectedTime.Hour ) * 60 + editing.Minute;
                //attribute.InnerText = minutes.ToString();
            }

            if (builtin_properties.Attributes["DocSecurity"] != null && builtin_properties.Attributes["DocSecurity"].InnerText == 8.ToString())
            {
                XmlNode dls = m_outXml.SelectSingleNode("DLS");
                XmlAttribute protection = m_outXml.CreateAttribute("ProtectionType");
                protection.InnerText = "AllowOnlyComments";
                dls.Attributes.Append(protection);
            }
        }

        /// <summary>
        /// Modifies the picture.
        /// </summary>
        /// <param name="node">The node.</param>
        private void ModifyPicture(XmlNode node)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.CurrencyDecimalSeparator = ".";
            double width = 0;
            double height = 0;
            XmlNode image = node.SelectSingleNode("image");
            bool isMetafile = (node.Attributes["IsMetafile"].InnerText == DEF_ATTRIBUTE_VALUE_TRUE.ToLower());
            XmlAttribute name = m_outXml.CreateAttribute("Name");
            name.InnerText = string.Format(DEF_IMAGE_NAME_FORMAT, m_imageCount, ((isMetafile) ? "m" : "o"));
            node.Attributes.Append(name);
            string style = string.Empty;

            Image img = null;

            if (image != null)
            {
                img = ReadImage(image, isMetafile);
            }

            if (node.Attributes["width"] != null)
                width = System.Convert.ToDouble(node.Attributes["width"].InnerText, provider);

            if (node.Attributes["WidthScale"] != null && !isMetafile)
            {
                string strWidthScale = node.Attributes["WidthScale"].InnerText;
                width = System.Convert.ToDouble(strWidthScale, provider) / 100 * img.Width;
            }

            style += string.Format("width: {0};", width);

            if (node.Attributes["height"] != null)
                height = System.Convert.ToDouble(node.Attributes["height"].InnerText, provider);

            if (node.Attributes["HeightScale"] != null && !isMetafile)
            {
                string strHeightScale = node.Attributes["HeightScale"].InnerText;
                height = System.Convert.ToDouble(strHeightScale, provider) / 100 * img.Height;
            }

            style += string.Format("height: {0}", height);

            XmlAttribute styleatr = m_outXml.CreateAttribute("style");
            styleatr.InnerText = style;
            node.Attributes.Append(styleatr);
            m_imageCount++;
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
        /// <param name="isMetaFile"></param>
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
            public string this[string name]
            {
                get
                {
                    string strReturn = string.Empty;

                    foreach (Bookmark bookmark in this)
                    {
                        if (bookmark.Name == name)
                        {
                            strReturn = bookmark.ID;
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
        /// <summary>
        /// 
        /// </summary>
        internal class Grid
        {
            #region Class members
            /// <summary>
            /// 
            /// </summary>
            private double m_dWidth = 0;
            #endregion

            #region Class properties
            /// <summary>
            /// Gets or sets the width.
            /// </summary>
            /// <value>The width.</value>
            public double Width
            {
                get
                {
                    return m_dWidth;
                }
                set
                {
                    m_dWidth = value;
                }
            }
            #endregion

            #region Class Initialize/Finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="Grid"/> class.
            /// </summary>
            public Grid()
            {
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="Grid"/> class.
            /// </summary>
            /// <param name="width">The width.</param>
            public Grid(double width)
            {
                m_dWidth = width;
            }
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        internal class GridList : List<Grid>
        {
            /// <summary>
            /// Adds the specified grid.
            /// </summary>
            /// <param name="grid">The grid.</param>
            /// <returns></returns>
            public int Add(Grid grid)
            {
                base.Add(grid);
                return base.Count - 1;
            }

            /// <summary>
            /// Adds the specified width.
            /// </summary>
            /// <param name="width">The width.</param>
            /// <returns></returns>
            public int Add(double width)
            {
                return Add(new Grid(width));
            }
            /// <summary>
            /// Gets the min.
            /// </summary>
            /// <param name="end">The end.</param>
            /// <returns></returns>
            public double GetMin(double end)
            {
                double sum = 0;

                foreach (Grid grid in this)
                {
                    sum += grid.Width;

                    if (sum > end)
                        return sum - end;
                }
                return 0;
            }
            /// <summary>
            /// Gets the collection count.
            /// </summary>
            /// <param name="start">The start.</param>
            /// <param name="end">The end.</param>
            /// <returns></returns>
            public int GetCollCount(double start, double end)
            {
                int result = 1;
                double sum = 0;

                foreach (Grid grid in this)
                {
                    sum += grid.Width;

                    if (end == sum)
                        return result;

                    if (sum > start)
                        result++;
                }

                return result - 1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal class RowList : List<GridList>
        {
            /// <summary>
            /// Adds the specified gridlist.
            /// </summary>
            /// <param name="gridlist">The gridlist.</param>
            /// <returns></returns>
            public int Add(GridList gridlist)
            {
                int index = gridlist.Count;
                base.Add(gridlist);
                return index;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal class TableGrid
        {
            #region Class members
            /// <summary>
            /// 
            /// </summary>
            private GridList tableGrid = new GridList();
            /// <summary>
            /// 
            /// </summary>
            private RowList rowList = new RowList();
            /// <summary>
            /// 
            /// </summary>
            private XmlNode m_table;
            /// <summary>
            /// 
            /// </summary>
            private NumberFormatInfo m_provider = new NumberFormatInfo();
            #endregion

            #region Class Initialize/Finalize methods
            /// <summary>
            /// Initializes a new instance of the <see cref="TableGrid"/> class.
            /// </summary>
            public TableGrid()
            {
                m_provider.CurrencyDecimalSeparator = ".";
            }
            #endregion

            #region Class Public Methods
            /// <summary>
            /// Parces the specified table.
            /// </summary>
            /// <param name="table">The table.</param>
            public void Parce(XmlNode table)
            {
                m_table = table;

                XmlNodeList rows = m_table.SelectNodes(string.Format(DEF_PATH_FORMAT, "rows", "row"));

                if (rows.Count > 0)
                {
                    foreach (XmlNode row in rows)
                    {
                        XmlNodeList colls = row.SelectNodes(string.Format(DEF_PATH_FORMAT, "cells", "cell"));

                        if (colls.Count > 0)
                        {
                            GridList collsgrid = new GridList();

                            foreach (XmlNode coll in colls)
                            {
                                collsgrid.Add(System.Convert.ToDouble(coll.Attributes["Width"].InnerText, m_provider));
                            }

                            rowList.Add(collsgrid);
                        }
                    }
                }
            }
            /// <summary>
            /// Saves this instance.
            /// </summary>
            public void Save()
            {
                ParceTableGrid();
                XmlElement tblGrid = m_table.OwnerDocument.CreateElement("tblGrid");

                foreach (Grid grid in tableGrid)
                {
                    XmlElement gridCol = m_table.OwnerDocument.CreateElement("gridCol");
                    gridCol.SetAttribute("w", (grid.Width * 20).ToString());
                    tblGrid.AppendChild(gridCol);
                }

                m_table.AppendChild(tblGrid);
                XmlNodeList rows = m_table.SelectNodes(string.Format(DEF_PATH_FORMAT, "rows", "row"));

                if (rows.Count > 0)
                {
                    foreach (XmlNode row in rows)
                    {
                        XmlNodeList colls = row.SelectNodes(string.Format(DEF_PATH_FORMAT, "cells", "cell"));
                        double end = 0;
                        double start = 0;

                        if (colls.Count > 0)
                        {
                            foreach (XmlNode coll in colls)
                            {
                                double width = System.Convert.ToDouble(coll.Attributes["Width"].InnerText, m_provider);
                                start = end;
                                end += width;
                                int collcount = tableGrid.GetCollCount(start, end);
                                if (collcount > 1)
                                {
                                    XmlAttribute collcnt = coll.OwnerDocument.CreateAttribute("collcount");
                                    collcnt.InnerText = collcount.ToString();
                                    coll.Attributes.Append(collcnt);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Class helper methods
            /// <summary>
            /// Parces the table grid.
            /// </summary>
            private void ParceTableGrid()
            {
                double min = 0;
                double end = 0;

                if (rowList.Count > 0)
                {
                    bool result = true;

                    while (result)
                    {
                        min = double.MaxValue;

                        foreach (GridList row in rowList)
                        {
                            double temp_min = row.GetMin(end);
                            min = (min > temp_min) ? temp_min : min;
                        }

                        if (min == 0)
                            break;

                        end += min;
                        tableGrid.Add(min);
                    }
                }
            }
            /// <summary>
            /// Adds the row.
            /// </summary>
            /// <param name="row">The row.</param>
            private void AddRow(GridList row)
            {
                rowList.Add(row);
            }
            #endregion
        }
        #endregion
    }
}

#endif