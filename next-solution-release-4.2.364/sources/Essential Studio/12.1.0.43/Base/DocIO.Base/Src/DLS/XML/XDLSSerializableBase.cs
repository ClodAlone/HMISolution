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

#region file using directives
using System;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.DLS.XML
{
    /// <summary>
    /// 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public abstract class XDLSSerializableBase : OwnerHolder, IXDLSSerializable
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private XDLSHolder m_XDLSHolder;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XDLSSerializableBase"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="entity">The entity.</param>
        protected XDLSSerializableBase(WordDocument doc, Entity entity)
            : base(doc, entity)
        {
        }
        #endregion

        #region IXDLSSerializable implement
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void IXDLSSerializable.WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            WriteXmlAttributes(writer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        void IXDLSSerializable.WriteXmlContent(IXDLSContentWriter writer)
        {
            XDLSHolder.WriteHolder(writer);
            WriteXmlContent(writer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        void IXDLSSerializable.ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            ReadXmlAttributes(reader);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        bool IXDLSSerializable.ReadXmlContent(IXDLSContentReader reader)
        {
            if (!XDLSHolder.ReadHolder(reader))
            {
                return ReadXmlContent(reader);
            }

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        XDLSHolder IXDLSSerializable.XDLSHolder
        {
            get
            {
                if (m_XDLSHolder == null)
                {
                    m_XDLSHolder = new XDLSHolder();
                }

                if (m_XDLSHolder.Cleared)
                {
                    m_XDLSHolder.Cleared = false;
                    InitXDLSHolder();
                }

                return m_XDLSHolder;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void IXDLSSerializable.RestoreReference(string name, int value)
        {
            RestoreReference(name, value);
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        internal object CloneInt()
        {
            return CloneImpl();
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal virtual void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        protected XDLSHolder XDLSHolder
        {
            get
            {
                return (this as IXDLSSerializable).XDLSHolder;
            }
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected virtual object CloneImpl()
        {
            XDLSSerializableBase clonObj = (XDLSSerializableBase)MemberwiseClone();
            clonObj.m_XDLSHolder = null;
            clonObj.SetOwner(clonObj.Document, null);

            return clonObj;
        }
        #endregion

        #region Class virtual / abstract methods
        /// <summary>
        /// Writes object data as xml attributes.
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
        }
        /// <summary>
        /// Writes object data as inside xml element.
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void WriteXmlContent(IXDLSContentWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
        }
        /// <summary>
        /// Reads object data from xml attributes.
        /// </summary>
        /// <param name="reader"></param>
        protected virtual void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
        }
        /// <summary>
        /// Reads object data from xml attributes.
        /// </summary>
        /// <param name="reader"></param>
        protected virtual bool ReadXmlContent(IXDLSContentReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return false;
        }
        /// <summary>
        /// Registers child objects in XDSL holder.
        /// </summary>
        protected virtual void InitXDLSHolder()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        protected virtual void RestoreReference(string name, int index)
        {
        }
        #endregion
    }
}