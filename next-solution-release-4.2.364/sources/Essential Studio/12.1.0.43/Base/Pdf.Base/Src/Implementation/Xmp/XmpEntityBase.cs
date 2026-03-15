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
using System.Xml;

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Base class for the xmp entities.
    /// </summary>
    public abstract class XmpEntityBase
    {
        #region Fields
        /// <summary>
        /// Parent node for this entity.
        /// </summary>
        private XmlNode m_xmlParent;

        /// <summary>
        /// Prefix of the entity namespace.
        /// </summary>
        private string m_entityPrefix;

        /// <summary>
        /// Local name of the entity.
        /// </summary>
        private string m_localName;

        /// <summary>
        /// Uri of the namespace.
        /// </summary>
        private string m_namespaceURI;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmpEntityBase"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="localName">Name of the local.</param>
        /// <param name="namespaceURI">The namespace URI.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal XmpEntityBase(XmlNode parent, string prefix, string localName, string namespaceURI)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            if (localName == null)
            {
                throw new ArgumentNullException("localName");
            }

            m_xmlParent = parent;
            m_entityPrefix = prefix;
            m_localName = localName;
            m_namespaceURI = namespaceURI;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Xml data of the entity.
        /// </summary>
        public XmlElement XmlData
        {
            get
            {
                return this.GetEntityXml();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="XmpEntityBase"/> is exists.
        /// </summary>
        /// <value><c>true</c> if exists; otherwise, <c>false</c>.</value>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal bool Exists
        {
            get
            {
                return CheckIfExists();
            }
        }

        /// <summary>
        /// Gets parent xml node for the entity.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal XmlNode EntityParent
        {
            get
            {
                return m_xmlParent;
            }
        }

        /// <summary>
        /// Gets namespace prefix of the entity.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal string EntityPrefix
        {
            get
            {
                return m_entityPrefix;
            }
        }

        /// <summary>
        /// Gets name of the entity's tag.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal string EntityName
        {
            get
            {
                return m_localName;
            }
        }

        /// <summary>
        /// Gets URI of the entity's namespace.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal string EntityNamespaceURI
        {
            get
            {
                return m_namespaceURI;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [suspend initialization].
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected bool SuspendInitialization
        {
            get
            {
                return GetSuspend();
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Initializes object.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void Initialize()
        {
            if (!SuspendInitialization)
            {
                if (!Exists)
                {
                    CreateEntity();
                }
            }
        }

        /// <summary>
        /// Checks whether entity already exists in the parent.
        /// </summary>
        /// <returns>True - if exists, False otherwise.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool CheckIfExists()
        {
            bool exists = GetEntityXml() != null;

            return exists;
        }

        /// <summary>
        /// Gets value indicating whether we have to suspend initialization.
        /// </summary>
        /// <returns>Value indicating whether we have to suspend initialization.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual bool GetSuspend()
        {
            return false;
        }

        /// <summary>
        /// Creates entity in the parent.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract void CreateEntity();

        /// <summary>
        /// Gets Xml data of the entity.
        /// </summary>
        /// <returns>XmlElement containing entity data.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract XmlElement GetEntityXml();
        #endregion

        #region Implementation
        /// <summary>
        /// Changes parent of the entity.
        /// </summary>
        /// <param name="parent">New Xml parent.</param>
        internal void SetXmlParent(XmlNode parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            m_xmlParent = parent;
        }
        #endregion
    }
}
