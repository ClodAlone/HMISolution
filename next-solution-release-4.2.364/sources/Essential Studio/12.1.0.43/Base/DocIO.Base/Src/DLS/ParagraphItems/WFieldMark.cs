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

#region File using directives
using System;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for FieldMark.
    /// </summary>
    public class WFieldMark : ParagraphItem
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private FieldMarkType m_fldMarkType;
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
                return EntityType.FieldMark;
            }
        }
        /// <summary>
        /// Gets character format of field mark.
        /// </summary>
        internal WCharacterFormat CharacterFormat
        {
            get
            {
                return m_charFormat;
            }
        }
        /// <summary>
        /// Gets/sets type of field mark. 
        /// </summary>
        public FieldMarkType Type
        {
            get
            {
                return m_fldMarkType;
            }
            set
            {
                m_fldMarkType = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WFieldMark"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal WFieldMark(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_charFormat = new WCharacterFormat(doc);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WFieldMark"/> class.
        /// </summary>
        /// <param name="fieldMark">The field mark.</param>
        /// <param name="doc">The doc.</param>
        protected internal WFieldMark(WFieldMark fieldMark, IWordDocument doc)
            : this(doc)
        {
            Type = fieldMark.Type;
            m_charFormat = (WCharacterFormat)fieldMark.CharacterFormat.CloneInt();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WFieldMark"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="type">The type.</param>
        internal WFieldMark(IWordDocument doc, FieldMarkType type)
            : base((WordDocument)doc)
        {
            m_fldMarkType = type;
            m_charFormat = new WCharacterFormat(doc);
        }
        #endregion

        #region XDLSSerializable overrides
//#if !SILVERLIGHT
        /// <summary>
        /// Reads object data from xml attributes.
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            if (reader.HasAttribute(XDLSConstants.FieldMarkTypeAttr))
            {
                m_fldMarkType = (FieldMarkType)reader.ReadEnum(XDLSConstants.FieldMarkTypeAttr, typeof(FieldMarkType));
            }
        }
        /// <summary>
        /// Writes object data as xml attributes.
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.TypeTag, ParagraphItemType.FieldMark);
            writer.WriteValue(XDLSConstants.FieldMarkTypeAttr, m_fldMarkType);
        }
        /// <summary>
        /// Registers child objects in XDSL holder.
        /// </summary>
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.CharacterFormatTag, m_charFormat);
        }
//#endif

        #endregion

        #region WidgetBase overrides
#if !SILVERLIGHT && !WP
        /// <summary>
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new Syncfusion.Layouting.LayoutInfo();
        }
#endif
        #endregion
    }
}
