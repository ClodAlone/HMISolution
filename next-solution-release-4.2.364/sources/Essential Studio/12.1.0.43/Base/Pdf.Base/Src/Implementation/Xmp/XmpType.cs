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
#if !NETFX_CORE
using System;
using System.Xml;

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Base class for the Xmp types.
    /// </summary>
    public abstract class XmpType : XmpEntityBase
    {
        #region Fields
        /// <summary>
        /// Parent Xmpmetadata.
        /// </summary>
        private XmpMetadata m_xmp;
        #endregion

        #region Properties
        /// <summary>
        /// Gets parent XmpMetadata.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected XmpMetadata Xmp
        {
            get
            {
                return this.m_xmp;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates xmp simple type instance.
        /// </summary>
        /// <param name="xmp">Parent XmpMetadata.</param>
        /// <param name="parent">Parent xml node.</param>
        /// <param name="prefix">Namespace prefix.</param>
        /// <param name="localName">Name of the tag.</param>
        /// <param name="namespaceURI">Namespace URI.</param>
        internal XmpType(XmpMetadata xmp, XmlNode parent, string prefix, string localName, string namespaceURI)
            : base(parent, prefix, localName, namespaceURI)
        {
            if (xmp == null)
            {
                throw new ArgumentNullException("xmp");
            }

            this.m_xmp = xmp;

            this.Initialize();
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Gets Xml data of the entity.
        /// </summary>
        /// <returns>XmlElement containing entity data.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override XmlElement GetEntityXml()
        {
            XmlNode node = null;
            if (this.m_xmp.isLoadedDocument)
            {
                if (EntityParent.InnerText != ""||EntityParent.InnerXml!="")
                {
                    node = EntityParent.SelectSingleNode("./" + EntityPrefix + ":" + EntityName,
                       this.Xmp.NamespaceManager);
                }
            }
            else
            {
                node = EntityParent.SelectSingleNode("./" + EntityPrefix + ":" + EntityName,
                   this.Xmp.NamespaceManager);
            }
            return (node as XmlElement);
        }

        /// <summary>
        /// Creates entity in the parent.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateEntity()
        {
            XmlElement element = this.Xmp.CreateElement(EntityPrefix, EntityName, EntityNamespaceURI);
            EntityParent.AppendChild(element);
        }
        #endregion
    }
}
#endif