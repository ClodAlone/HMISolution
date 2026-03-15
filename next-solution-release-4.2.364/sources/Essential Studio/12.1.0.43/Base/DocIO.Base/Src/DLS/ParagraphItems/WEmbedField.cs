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
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.OLEObject;
#if !SILVERLIGHT && !WP
using Syncfusion.Layouting;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents an embeded field.
    /// </summary>
    internal class WEmbedField : WField
    {
        #region Class fields
        protected internal int m_storagePicLocation = 0;
        protected internal bool m_isOle2 = false;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.EmbededField;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int StoragePicLocation
        {
            get
            {
                return m_storagePicLocation;
            }
            set
            {
                m_storagePicLocation = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsOle2
        {
            get
            {
                return m_isOle2;
            }
            set
            {
                m_isOle2 = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        internal WEmbedField(IWordDocument doc)
            : base(doc)
        {
            m_paraItemType = ParagraphItemType.EmbedField;
        }
        #endregion

        #region XDLSSerializationBase overrides
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            base.InitXDLSHolder();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.EmbedFieldStorageNameAttr))
            {
                m_storagePicLocation = reader.ReadInt(XDLSConstants.EmbedFieldStorageNameAttr);
                m_isOle2 = reader.ReadBoolean(XDLSConstants.EmbedObjectIsOle2Attr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (m_storagePicLocation > 0)
            {
                writer.WriteValue(XDLSConstants.EmbedFieldStorageNameAttr, m_storagePicLocation);
                writer.WriteValue(XDLSConstants.EmbedObjectIsOle2Attr, m_isOle2);
            }
        }
//#endif
        #endregion

        #region Class overrides
        /// <summary>
        /// Clone values of all fields of current embedded field
        /// </summary>
        /// <returns>
        /// new WEmbedField, containing the same info as in current
        /// </returns>
        protected override object CloneImpl()
        {
            WEmbedField ef = (WEmbedField)base.CloneImpl();
            return ef;
        }
        #endregion
    }
    /// <summary>
    /// Class represents form control field.
    /// </summary>
    public class WControlField : WField
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private int m_storagePicLocation = 0;
        private OLEObject m_oleObject;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.ControlField;
            }
        }
        /// <summary>
        /// Gets or sets the storage location.
        /// </summary>
        /// <value>The storage pic location.</value>
        internal int StoragePicLocation
        {
            get
            {
                return m_storagePicLocation;
            }
            set
            {
                m_storagePicLocation = value;
            }
        }
        /// <summary>
        /// Gets the OLE object.
        /// </summary>
        /// <value>The OLE object.</value>
        internal OLEObject OleObject
        {
            get
            {
                if (m_oleObject == null)
                    m_oleObject = new OLEObject();
                return m_oleObject;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WControlField"/> class.
        /// </summary>
        /// <param name="doc"></param>
        internal WControlField(IWordDocument doc)
            : base(doc)
        {
            m_paraItemType = ParagraphItemType.ControlField;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Clone values of all fields of current embedded field
        /// </summary>
        /// <returns>
        /// new WEmbedField, containing the same info as in current
        /// </returns>
        protected override object CloneImpl()
        {
            WControlField controlFld = (WControlField)base.CloneImpl();
            if (m_oleObject != null)
            {
                controlFld.m_oleObject = m_oleObject.Clone();
                controlFld.m_storagePicLocation = WOleObject.NextOleObjId;
                controlFld.CharacterFormat.PicLocation = controlFld.m_storagePicLocation;
                controlFld.m_oleObject.Storage.StorageName = "_" + controlFld.m_storagePicLocation.ToString();
            }
            return controlFld;
        }
        /// <summary>
        /// Clones object pool of source document to destination document.
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            base.CloneRelationsTo(doc, nextOwner);
        }
        #endregion
    }
}
