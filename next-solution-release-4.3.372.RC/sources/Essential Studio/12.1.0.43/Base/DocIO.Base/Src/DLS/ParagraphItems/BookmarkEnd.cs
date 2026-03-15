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
#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.Rendering;
using Syncfusion.Layouting;
#endif
using IWParagraph = Syncfusion.DocIO.DLS.IWParagraph;
using System.Windows;
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents the end position of the <see cref="Syncfusion.DocIO.DLS.Bookmark"/>
    /// in the document.
    /// </summary>
    public class BookmarkEnd
      : ParagraphItem
#if !SILVERLIGHT && !WP
      , ILeafWidget
#endif
    {
        #region Fields
        private string m_strName = "";
        private bool m_isCellGroupBkmk;
        /// <summary>
        /// Specifies whether the Bookmark is detached (For both cases either cloned or removed from collection) from the Bookmarks collection.
        /// </summary>
        internal bool m_isDetached = false;
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
                return EntityType.BookmarkEnd;
            }
        }
        /// <summary>
        /// Gets the bookmark name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return m_strName;
            }
        }
        /// <summary>
        /// Gets/sets IsCellGroupBkmk value.
        /// </summary>
        internal bool IsCellGroupBkmk
        {
            get
            {
                return m_isCellGroupBkmk;
            }
            set
            {
                m_isCellGroupBkmk = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkEnd"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal BookmarkEnd(WordDocument doc)
            : this(doc, "")
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkEnd"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="name">The name.</param>
        public BookmarkEnd(IWordDocument document, string name)
            : base((WordDocument)document)
        {
            SetName(name);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the name.
        /// </summary>
        /// <param name="name">The name.</param>
        internal void SetName(string name)
        {
            m_strName = name.Replace('-', '_');
        }
        /// <summary>
        /// Attaches to paragraph.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="itemPos"></param>
        internal override void Attach(WParagraph owner, int itemPos)
        {
            base.Attach(owner, itemPos);

            if (!DeepDetached)
            {
                Document.Bookmarks.AttachBookmarkEnd(this);
                m_isDetached = false;
            }
            else
            {
                m_isDetached = true;
            }
        }
        /// <summary>
        /// Detaches from owner.
        /// </summary>
        internal override void Detach()
        {
            base.Detach();

            if (!DeepDetached)
            {
                Bookmark bkmk = Document.Bookmarks.FindByName(Name);

                if (bkmk != null)
                {
                    bkmk.SetEnd(null);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override void CloneCommit()
        {
            if (m_isDetached)
            {
                Document.Bookmarks.AttachBookmarkEnd(this);
                m_isDetached = false;
            }
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            BookmarkEnd be = (BookmarkEnd)base.CloneImpl();
            be.m_isDetached = true;
            return be;
        }
        #endregion
//#if !SILVERLIGHT
        #region Implementation / xml

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            writer.WriteValue(PropertyNames.Type, ParagraphItemType.BookmarkEnd);
            writer.WriteValue(XDLSConstants.BookMarkNameAttr, Name);

            if (IsCellGroupBkmk)
            {
                writer.WriteValue(XDLSConstants.IsCellGroupBkmkAttr, IsCellGroupBkmk);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            m_strName = reader.ReadString(XDLSConstants.BookMarkNameAttr);
            Document.Bookmarks.AttachBookmarkEnd(this);

            if (reader.HasAttribute(XDLSConstants.IsCellGroupBkmkAttr))
            {
                IsCellGroupBkmk = reader.ReadBoolean(XDLSConstants.IsCellGroupBkmkAttr);
            }
        }
        #endregion

#if !SILVERLIGHT && !WP
        #region Implementation / layout
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo(ChildrenLayoutDirection.Horizontal);
            m_layoutInfo.IsClipped = (this.GetOwnerParagraph() as IWidget).LayoutInfo.IsClipped;
        }
        /// <summary>
        /// Measures the specified Custom Graphics.
        /// </summary>
        /// <param name="cg">The Custom Graphics.</param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        SizeF ILeafWidget.Measure(DrawingContext dc)
        {
            SizeF size = new SizeF();
            WParagraph ownerParagraph = this.GetOwnerParagraph();
            if (ownerParagraph.IsNeedToMeasureBookMarkSize)
                size.Height = (ownerParagraph as IWidget).LayoutInfo.Size.Height;
            return size;
        }
        #endregion
#endif
//#endif
    }
}
