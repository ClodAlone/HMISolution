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
using System.Collections;
using System.IO;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.DocIO.ReaderWriter.Escher;

#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.Rendering;
using Syncfusion.Layouting;
using System.Drawing;
#endif

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for ShapeObject.
    /// </summary>
    public class ShapeObject : ParagraphItem
#if !SILVERLIGHT && !WP
    , ILeafWidget
#endif
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private FileShapeAddress m_fspa;
        /// <summary>
        /// 
        /// </summary>
        private WTextBoxCollection m_textBoxColl;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isHeader;
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
                return EntityType.Shape;
            }
        }
        /// <summary>
        /// Get/set FSPA for ShapeObject.
        /// </summary>
        internal FileShapeAddress FSPA
        {
            get
            {
                return m_fspa;
            }
            set
            {
                m_fspa = value;
            }
        }
        /// <summary>
        /// Gets/sets ShapeObject's main autoshape collection.
        /// </summary>
        internal WTextBoxCollection AutoShapeTextCollection
        {
            get
            {
                return m_textBoxColl;
            }
        }
        /// <summary>
        /// Get/set value which defines whenever current autoshape
        /// is in header/footer subdocument.
        /// </summary>
        internal bool IsHeaderAutoShape
        {
            get
            {
                return m_isHeader;
            }
            set
            {
                m_isHeader = value;
            }
        }
        /// <summary>
        /// Gets shape object's character format.
        /// </summary>
        internal WCharacterFormat CharacterFormat
        {
            get
            {
                return m_charFormat;
            }
        }

        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal ShapeObject(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_fspa = new FileShapeAddress();
            m_textBoxColl = new WTextBoxCollection(doc);
            m_charFormat = new WCharacterFormat(doc);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            foreach (WTextBox textbox in this.AutoShapeTextCollection)
            {
                textbox.AddSelf();
            }
        }
        /// <summary>
        /// Attaches to paragraph.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="itemPos"></param>
        internal override void Attach(WParagraph owner, int itemPos)
        {
            base.Attach(owner, itemPos);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            foreach (WTextBox textbox in this.AutoShapeTextCollection)
            {
                foreach (Entity ent in textbox.ChildEntities)
                {
                    ent.CloneRelationsTo(doc, nextOwner);
                    ent.SetOwner(doc);
                }
            }
            Document.CloneShapeEscher(doc, this);
            this.Cloned = false;
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            ShapeObject so = (ShapeObject)base.CloneImpl();

            so.m_textBoxColl = new WTextBoxCollection(Document);
            m_textBoxColl.CloneTo(so.m_textBoxColl);

            if (FSPA != null)
            {
                so.m_fspa = FSPA.Clone();
            }

            so.Cloned = true;
            return so;
        }
//#if !SILVERLIGHT
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo(ChildrenLayoutDirection.Horizontal);
            m_layoutInfo.IsClipped = true;
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.TypeTag, ParagraphItemType.ShapeObject);
            writer.WriteValue(XDLSConstants.ShapeIdentAttr, m_fspa.Spid);
            writer.WriteValue(XDLSConstants.ShapeIsBelowTextAttr, m_fspa.IsBelowText);
            writer.WriteValue(XDLSConstants.ShapeHorizOriginAttr, m_fspa.RelHrzPos);
            writer.WriteValue(XDLSConstants.ShapeVertOriginAttr, m_fspa.RelVrtPos);
            writer.WriteValue(XDLSConstants.ShapeWrappingStyleAttr, m_fspa.TextWrappingStyle);
            writer.WriteValue(XDLSConstants.ShapeWrappingTypeAttr, m_fspa.TextWrappingType);
            writer.WriteValue(XDLSConstants.ShapeHorizPositionAttr, m_fspa.XaLeft);
            writer.WriteValue(XDLSConstants.ShapeVertPositionAttr, m_fspa.YaTop);
            writer.WriteValue(XDLSConstants.ShapeTextBoxCountAttr, m_fspa.TxbxCount);
            writer.WriteValue(XDLSConstants.ShapeHeightAttr, m_fspa.Height);
            writer.WriteValue(XDLSConstants.ShapeWidthAttr, m_fspa.Width);
            writer.WriteValue(XDLSConstants.ShapeIsHeaderAttr, m_isHeader);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.ShapeIdentAttr))
            {
                m_fspa.Spid = reader.ReadInt(XDLSConstants.ShapeIdentAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ShapeIsBelowTextAttr))
            {
                m_fspa.IsBelowText = reader.ReadBoolean(XDLSConstants.ShapeIsBelowTextAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ShapeHorizOriginAttr))
            {
                m_fspa.RelHrzPos = (HorizontalOrigin)
                  reader.ReadEnum(XDLSConstants.ShapeHorizOriginAttr, typeof(HorizontalOrigin));
            }
            if (reader.HasAttribute(XDLSConstants.ShapeVertOriginAttr))
            {
                m_fspa.RelVrtPos = (VerticalOrigin)
                  reader.ReadEnum(XDLSConstants.ShapeVertOriginAttr, typeof(VerticalOrigin));
            }
            if (reader.HasAttribute(XDLSConstants.ShapeWrappingStyleAttr))
            {
                m_fspa.TextWrappingStyle = (TextWrappingStyle)
                  reader.ReadEnum(XDLSConstants.ShapeWrappingStyleAttr, typeof(TextWrappingStyle));
            }
            if (reader.HasAttribute(XDLSConstants.ShapeWrappingTypeAttr))
            {
                m_fspa.TextWrappingType = (TextWrappingType)
                  reader.ReadEnum(XDLSConstants.ShapeWrappingTypeAttr, typeof(TextWrappingType));
            }
            if (reader.HasAttribute(XDLSConstants.ShapeHorizPositionAttr))
            {
                m_fspa.XaLeft = reader.ReadInt(XDLSConstants.ShapeHorizPositionAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ShapeVertPositionAttr))
            {
                m_fspa.YaTop = reader.ReadInt(XDLSConstants.ShapeVertPositionAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ShapeTextBoxCountAttr))
            {
                m_fspa.TxbxCount = reader.ReadInt(XDLSConstants.ShapeTextBoxCountAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ShapeHeightAttr))
            {
                m_fspa.Height = reader.ReadInt(XDLSConstants.ShapeHeightAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ShapeWidthAttr))
            {
                m_fspa.Width = reader.ReadInt(XDLSConstants.ShapeWidthAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ShapeIsHeaderAttr))
            {
                m_isHeader = reader.ReadBoolean(XDLSConstants.ShapeIsHeaderAttr);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            base.InitXDLSHolder();

            XDLSHolder.AddElement(XDLSConstants.TextBoxesTag, m_textBoxColl);
            XDLSHolder.AddElement(XDLSConstants.CharacterFormatTag, m_charFormat);
        }
//#endif
        #endregion

        #region ILeafWidget Members
#if !SILVERLIGHT && !WP
        SizeF ILeafWidget.Measure(DrawingContext dc)
        {
            return new SizeF();
        }
#endif

        #endregion
    }
}
