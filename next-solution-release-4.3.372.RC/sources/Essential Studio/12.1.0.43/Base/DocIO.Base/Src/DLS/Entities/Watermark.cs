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
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for Watermark.
    /// </summary>
    public class Watermark : ParagraphItem
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private WatermarkType m_type;
        private bool m_placeOnFirstPg = true;
        private bool m_placeOnOddPg = true;
        private bool m_placeOnEvenPg = true;
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
                return EntityType.Undefined;
            }
        }
        /// <summary>
        /// Gets the watermark type.
        /// </summary>
        public WatermarkType Type
        {
            get
            {
                return m_type;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Watermark"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        internal Watermark(WatermarkType type)
            : base(null)
        {
            m_type = type;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Watermark"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="type">The type.</param>
        internal Watermark(WordDocument doc, WatermarkType type)
            : base(doc)
        {
            m_type = type;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Removes the self.
        /// </summary>
        internal override void RemoveSelf()
        {
        }
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.WatermarkTypeAttr, m_type);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(Syncfusion.DocIO.DLS.XML.IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.WatermarkTypeAttr))
            {
                m_type = (WatermarkType)reader.ReadEnum(XDLSConstants.WatermarkTypeAttr, typeof(WatermarkType));
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new Syncfusion.Layouting.LayoutInfo();
        }
#endif
//#endif
        #endregion
    }
}
