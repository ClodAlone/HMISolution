#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Xml;

namespace Syncfusion.SVG.IO
{
    /// <summary>
    /// Class containing SVG Document.
    /// </summary>
    public class SvgDocument : Element
    {
        #region Constants
        private const string c_strSVG_NAMESPACE = "http://www.w3.org/2000/svg";
        private const string c_strXLINKNS = "xmlns:xlink";
        private const string c_strXMLNS = "xmlns";
        private const string c_strSVG = "svg";
        private const int MAX_SIZE_VALUE = 30;
        private const string XLINK_NAMESPACE_URL = "http://www.w3.org/1999/xlink";
        private const string PREFIX_XLINK = "xlink:";
        #endregion

        #region Members
        private long m_currId = 0;
        private SvgElement m_svg = null;
        private DefsElement m_defs = null;
        #endregion

        #region Proprties
        /// <summary>
        /// Gets or sets the current id.
        /// </summary>
        /// <value>The current id.</value>
        internal long CurrentId
        {
            get
            {
                return m_currId;
            }
            set
            {
                m_currId = value;
            }
        }

        /// <summary>
        /// Gets the SVG.
        /// </summary>
        /// <value>The SVG.</value>
        public SvgElement Svg
        {
            get
            {
                if (m_svg == null)
                {
                    m_svg = new SvgElement();
                    this.AddChild(m_svg);
                }

                return m_svg;
            }
        }

        /// <summary>
        /// Gets the defs.
        /// </summary>
        /// <value>The defs.</value>
        public DefsElement Defs
        {
            get
            {
                if (m_defs == null)
                {
                    m_defs = new DefsElement();
                    Svg.AddChild(m_defs);
                }

                return m_defs;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SvgDocument"/> class.
        /// </summary>
        public SvgDocument()
        {
            m_document = this;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Finds the element.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The element.</returns>
        public Element FindElement(string id)
        {
            return FindElement(id, this);
        }

        /// <summary>
        /// Saves the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Save(string filename)
        {
            XmlDocument doc = new XmlDocument();

            foreach (Element chld in Children)
            {
                SaveElement(chld, doc, doc);
            }

            OptimazeDocument(doc);
            doc.Save(filename);
        }

        /// <summary>
        /// Loads the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Load(string filename)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);

            foreach (XmlNode el in xmldoc.DocumentElement.ChildNodes)
            {
                LoadElements(el, this);
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Finds the element.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="elem">The elem.</param>
        /// <returns>The element.</returns>
        private Element FindElement(string id, Element elem)
        {
            Element res = null;

            if (elem.Id == id)
            {
                res = elem;
            }
            else
            {
                foreach (Element el in elem.Children)
                {
                    res = FindElement(id, el);

                    if (res != null)
                    {
                        break;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Saves the element.
        /// </summary>
        /// <param name="elem">The element.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="doc">The doc.</param>
        private void SaveElement(Element elem, XmlNode parent, XmlDocument doc)
        {
            XmlElement node = doc.CreateElement(elem.Name);

            if (elem.Name == c_strSVG)
            {
                XmlAttribute attr = doc.CreateAttribute(c_strXMLNS);
                attr.Value = c_strSVG_NAMESPACE;
                node.Attributes.Append(attr);

                attr = doc.CreateAttribute(c_strXLINKNS);
                attr.Value = XLINK_NAMESPACE_URL;
                node.Attributes.Append(attr);
            }

            if (elem.Text != string.Empty)
            {
                node.InnerText = elem.Text;
            }

            foreach (object key in elem.Attributes.Keys)
            {
                string skey = key.ToString();

                if (skey.IndexOf(PREFIX_XLINK) > -1)
                {
                    XmlAttribute attr = doc.CreateAttribute(skey, XLINK_NAMESPACE_URL);
                    attr.Value = elem.Attributes[key].ToString();
                    node.Attributes.Append(attr);
                }
                else
                {
                    node.SetAttribute(key.ToString(), elem.Attributes[key].ToString());
                }
            }

            foreach (Element chld in elem.Children)
            {
                SaveElement(chld, node, doc);
            }

            parent.AppendChild(node);
        }

        /// <summary>
        /// Loads the elements.
        /// </summary>
        /// <param name="elem">The xml element.</param>
        /// <param name="parent">The parent.</param>
        private void LoadElements(XmlNode elem, Element parent)
        {
            Element newElem = null;

            // Very big switch - Svg element types
            switch (elem.Name)
            {
                case SVG.NAME_DEFS:
                    newElem = new DefsElement();
                    break;
                case SVG.NAME_ELLIPSE:
                    newElem = new EllipseElement();
                    break;
                case SVG.NAME_CIRCLE:
                    newElem = new CircleElement();
                    break;
                case SVG.NAME_G:
                    newElem = new GElement();
                    break;
                case SVG.NAME_IMAGE:
                    newElem = new ImageElement();
                    break;
                case SVG.NAME_LINE:
                    newElem = new LineElement();
                    break;
                case SVG.NAME_LINEAR_GRADIENT:
                    newElem = new LinearGradientElement();
                    break;
                case SVG.NAME_PATH:
                    newElem = new PathElement();
                    break;
                case SVG.NAME_PATTERN:
                    newElem = new PatternElement();
                    break;
                case SVG.NAME_POLYGON:
                    newElem = new PolygonElement();
                    break;
                case SVG.NAME_POLYLINE:
                    newElem = new PolylineElement();
                    break;
                case SVG.NAME_RADIAL_GRADIENT:
                    break;
                case SVG.NAME_RECT:
                    newElem = new RectElement();
                    break;
                case SVG.NAME_STOP:
                    newElem = new StopElement();
                    break;
                case SVG.NAME_SVG:
                    newElem = new SvgElement();
                    break;
                case SVG.NAME_TEXT:
                    newElem = new TextElement();
                    break;
            }

            if (newElem != null)
            {
                parent.AddChild(newElem);
                newElem.ParseXml(elem);

                foreach (XmlNode el in elem.ChildNodes)
                {
                    LoadElements(el, newElem);
                }
            }
        }

        /// <summary>
        /// Adds child element.
        /// </summary>
        /// <param name="elem">The element.</param>
        /// <returns>true, if can add child.</returns>
        protected override bool CanAddChild(Element elem)
        {
            bool res = true;

            if (elem is SvgElement)
            {
                m_svg = elem as SvgElement;
            }
            else if (elem is DefsElement)
            {
                m_defs = elem as DefsElement;
            }

            return res;
        }
        private void OptimazeDocument(XmlDocument doc)
        {
            string all = string.Empty;

            XmlDocumentType xmlDocType = doc.CreateDocumentType("svg", null, null, all);
            doc.PrependChild(xmlDocType);
        }
        private Hashtable CreateEntity(XmlNode node, ref int id)
        {
            Hashtable res = new Hashtable();

            if (node.Attributes != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Value.Length > MAX_SIZE_VALUE)
                    {
                        res.Add(id.ToString(), attr.Value);
                        id++;
                    }
                }
            }

            foreach (XmlNode cld in node.ChildNodes)
            {
                Hashtable cr = CreateEntity(cld, ref id);

                foreach (object key in cr.Keys)
                {
                    if (!res.ContainsValue(cr[key]))
                    {
                        res.Add(key, cr[key]);
                    }
                }
            }

            return res;
        }
        private void SetEntityToNode(XmlNode node, Hashtable ents)
        {
            if (node.Attributes != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    foreach (object key in ents.Keys)
                    {
                        if (ents[key].Equals(attr.Value))
                        {
                            attr.Value = string.Empty;
                            attr.AppendChild(node.OwnerDocument.CreateEntityReference("E" + key.ToString()));
                            break;
                        }
                    }
                }
            }

            foreach (XmlNode chld in node.ChildNodes)
            {
                SetEntityToNode(chld, ents);
            }
        }
        #endregion
    }
}
