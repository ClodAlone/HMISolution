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

using System;
using System.Collections;
using System.Drawing;
using System.Xml;

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
    /// <summary>
    /// Base implementation of <see cref="IElement"/> interface.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public abstract class Element : IElement
    {
        #region Helper classes
        /// <summary>
        /// Represents the indexer of the element children collection.
        /// </summary>
        public sealed class ElementsList : IEnumerable
        {
            #region Members
            private ArrayList m_list;
            #endregion

            #region Proprties
            /// <summary>
            /// Gets the <see cref="Syncfusion.Windows.Forms.Chart.SvgBase.Element"/> at the specified index.
            /// </summary>
            /// <value></value>
            public Element this[int index]
            {
                get
                {
                    return m_list[index] as Element;
                }
            }

            /// <summary>
            /// Gets the count.
            /// </summary>
            /// <value>The count.</value>
            public int Count
            {
                get
                {
                    return m_list.Count;
                }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="ElementsList"/> class.
            /// </summary>
            /// <param name="list">The list.</param>
            internal ElementsList(ArrayList list)
            {
                m_list = list;
            }
            #endregion

            #region IEnumerable Members
            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator GetEnumerator()
            {
                return m_list.GetEnumerator();
            }
            #endregion
        }
        #endregion

        #region Members
        private readonly string m_name;
        private string m_text = "";
        private Element m_parent = null;
        private ArrayList m_children = new ArrayList();
        private Hashtable m_attributes = new Hashtable();
        private SvgDocument m_document = null;
        private ElementsList m_elemList = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of element.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return m_name;
            }
        }

        /// <summary>
        /// Gets the inner elements.
        /// </summary>
        /// <value>The inner elements.</value>
        public ElementsList Children
        {
            get
            {
                if (m_elemList == null)
                {
                    m_elemList = new ElementsList(m_children);
                }

                return m_elemList;
            }
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public Hashtable Attributes
        {
            get
            {
                return m_attributes;
            }
        }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        /// <value>The parent element.</value>
        public Element Parent
        {
            get
            {
                return m_parent;
            }
        }

        /// <summary>
        /// Gets or sets the element identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id
        {
            get
            {
                return (string)GetAttribute(SVG.ATTR_ID, "");
            }

            set
            {
                m_attributes[SVG.ATTR_ID] = value;
            }
        }

        /// <summary>
        /// Gets the owner document.
        /// </summary>
        /// <value>The owner document.</value>
        public SvgDocument OwnerDocument
        {
            get
            {
                return m_document;
            }
        }

        /// <summary>
        /// Gets or sets the inner text of element.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return m_text;
            }

            set
            {
                m_text = value;
            }
        }
        #endregion

        #region Constrcutor
        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="name">The name of element.</param>
        public Element(string name)
        {
            m_name = name;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void AddChild(Element value)
        {
            if (CanAddChild(value))
            {
                m_children.Add(value);
                value.m_parent = this;
                value.SetDocument(m_document);
            }
        }

        /// <summary>
        /// Draws the specified g.
        /// </summary>
        /// <param name="g">The g.</param>
        public virtual void Draw(Graphics g)
        {
            foreach (SvgElement element in m_children)
            {
                element.Draw(g);
            }
        }

        /// <summary>
        /// Sets the document.
        /// </summary>
        /// <param name="doc">The <see cref="SvgDocument"/>.</param>
        internal void SetDocument(SvgDocument doc)
        {
            m_document = doc;

            if (doc != null)
            {
                Id = doc.CurrentId.ToString();
                doc.CurrentId++;

                foreach (Element el in Children)
                {
                    el.SetDocument(doc);
                }
            }
        }

        /// <summary>
        /// Parses the XML document.
        /// </summary>
        /// <param name="node">The <see cref="XmlNode"/>.</param>
        internal virtual void ParseXml(XmlNode node)
        {
            Text = node.InnerText;

            if (node.Attributes[SVG.ATTR_ID] != null)
            {
                Id = node.Attributes[SVG.ATTR_ID].Value;
            }
        }

        /// <summary>
        /// Writes the XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(m_name);

            this.WriteXmlAttributes(writer);

            writer.WriteString(this.Text);
            writer.WriteString(Environment.NewLine);

            foreach (Element child in m_children)
            {
                child.WriteXml(writer);
            }

            writer.WriteEndElement();
        }
        #endregion

        #region Helper methdos
        /// <summary>
        /// Writes the attributes to XML document.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/>.</param>
        protected virtual void WriteXmlAttributes(XmlWriter writer)
        {
            foreach (string attrKey in m_attributes.Keys)
            {
                writer.WriteAttributeString(attrKey, m_attributes[attrKey].ToString());
            }
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defValue">The default value.</param>
        /// <returns>Returns object of the specified key.</returns>
        protected object GetAttribute(object key, object defValue)
        {
            object res = m_attributes[key];

            if (res == null)
            {
                res = defValue;
            }

            return res;
        }

        /// <summary>
        /// Sets the value of attribute.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="defValue">The default value.</param>
        protected void SetAttribute(object key, object value, object defValue)
        {
            if (value == defValue)
            {
                m_attributes.Remove(key);
            }
            else
            {
                m_attributes[key] = value;
            }
        }

        /// <summary>
        /// Determines whether the specified child is allowed to add to the this element.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns>
        /// 	<c>true</c> if the specified child is allowed to add to the this element; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanAddChild(Element elem)
        {
            return elem != null;
        }
        #endregion
    }
}
