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
    /// Element class.
    /// </summary>
    public abstract class Element : IElement
    {
        #region Helper classes
        /// <summary>
        /// Elements list class.
        /// </summary>
        public class ElementsList : IEnumerable
        {
            #region Members
            private ArrayList m_list;
            #endregion

            #region Proprties
            /// <summary>
            /// Gets the <see cref="Syncfusion.SVG.IO.Element"/> at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <value>The element</value>
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
            public ElementsList(ArrayList list)
            {
                m_list = list;
            }
            #endregion

            #region IEnumerable Members
            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator GetEnumerator()
            {
                return m_list.GetEnumerator();
            }
            #endregion
        }
        #endregion

        #region Members
        protected string m_name;
        protected string m_text = string.Empty;
        protected Element m_parent = null;
        protected ArrayList m_children = new ArrayList();
        protected Hashtable m_attributes = new Hashtable();
        protected SvgDocument m_document = null;
        private ElementsList m_elemList = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name.
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
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
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
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public Element Parent
        {
            get
            {
                return m_parent;
            }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id
        {
            get
            {
                return (string)GetAttribute(SVG.ATTR_ID, string.Empty);
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
        /// Gets or sets the text.
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
        public Element()
        {
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
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="g">The graphics.</param>
        public virtual void Draw(Graphics g)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Draw(g);
            }
        }

        /// <summary>
        /// Sets the document.
        /// </summary>
        /// <param name="doc">The doc.</param>
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
        /// Parses the XML.
        /// </summary>
        /// <param name="node">The node.</param>
        internal virtual void ParseXml(XmlNode node)
        {
            Text = node.InnerText;

            if (node.Attributes[SVG.ATTR_ID] != null)
            {
                Id = node.Attributes[SVG.ATTR_ID].Value;
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defValue">The def value.</param>
        /// <returns>The object.</returns>
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
        /// Sets the attribute.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="defValue">The def value.</param>
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
        /// Determines whether this instance [can add child] the specified elem.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns>
        /// <c>true</c> if this instance [can add child] the specified elem; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanAddChild(Element elem)
        {
            return true;
        }
        #endregion
    }
}
