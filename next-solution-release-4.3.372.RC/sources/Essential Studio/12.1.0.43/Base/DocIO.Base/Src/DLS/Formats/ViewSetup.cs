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
    /// Summary description for ViewSetup.
    /// </summary>
    public class ViewSetup : XDLSSerializableBase
    {
        #region Class constants
        /// <summary>
        /// Constant value for Zoom.
        /// </summary>
        public const int DEF_ZOOMING = 100;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private ZoomType m_zoomType;
        /// <summary>
        /// 
        /// </summary>
        private int m_zoomPercent;
        /// <summary>
        /// 
        /// </summary>
        private DocumentViewType m_docViewType;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets / sets zooming value in percents
        /// </summary>
        /// <value>The zoom percent.</value>
        public int ZoomPercent
        {
            get
            {
                return m_zoomPercent;
            }
            set
            {
                if (value < 10 || value > 500)
                {
                    throw new ArgumentOutOfRangeException("Zoom percentage must be between 10 and 500 percent.");
                }
                m_zoomPercent = value;
            }
        }
        /// <summary>
        /// Gets / sets zooming type
        /// </summary>
        /// <value>The type of the zoom.</value>
        public ZoomType ZoomType
        {
            get
            {
                return m_zoomType;
            }
            set
            {
                m_zoomType = value;
            }
        }
        /// <summary>
        /// Gets / sets document view mode
        /// </summary>
        /// <value>The type of the document view.</value>
        public DocumentViewType DocumentViewType
        {
            get
            {
                return m_docViewType;
            }
            set
            {
                m_docViewType = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Creates PageSetup object for specified document.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public ViewSetup(IWordDocument doc)
            : base((WordDocument)doc, null)
        {
            m_zoomType = ZoomType.None;
            m_docViewType = DocumentViewType.PrintLayout;
            m_zoomPercent = DEF_ZOOMING;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        internal ViewSetup Clone(WordDocument doc)
        {
            ViewSetup vs = (ViewSetup)CloneImpl();
            vs.SetOwner(doc);
            return vs;
        }
        /// <summary>
        /// Sets the zoom percent.
        /// Validates the Zoom percentage value while parsing the document.
        /// </summary>
        /// <param name="value">The value.</param>
        internal void SetZoomPercent(int value)
        {
            if (value == 0)
                value = DEF_ZOOMING;
            else if (value < 10)
                value = 10;
            else if (value > 500)
                value = 500;
            if (value >= 10 && value <= 500)
                m_zoomPercent = value;
        }
        #endregion

        #region Implementation / xml
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (ZoomPercent != DEF_ZOOMING)
            {
                writer.WriteValue(XDLSConstants.ViewSetupZoomPercentAttr, ZoomPercent);
            }
            if (ZoomType != ZoomType.None)
            {
                writer.WriteValue(XDLSConstants.ViewSetupZoomTypeAttr, ZoomType);
            }
            if (DocumentViewType != DocumentViewType.PrintLayout)
            {
                writer.WriteValue(XDLSConstants.ViewSetupViewTypeAttr, DocumentViewType);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.ViewSetupZoomPercentAttr))
            {
                ZoomPercent = reader.ReadInt(XDLSConstants.ViewSetupZoomPercentAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ViewSetupZoomTypeAttr))
            {
                ZoomType = (ZoomType)reader.ReadEnum(XDLSConstants.ViewSetupZoomTypeAttr, typeof(ZoomType));
            }
            if (reader.HasAttribute(XDLSConstants.ViewSetupViewTypeAttr))
            {
                DocumentViewType = (DocumentViewType)reader.ReadEnum(XDLSConstants.ViewSetupViewTypeAttr, typeof(DocumentViewType));
            }

        }
//#endif
        #endregion
    }
}
