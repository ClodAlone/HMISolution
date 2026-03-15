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
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents the start position of the <see cref="Syncfusion.DocIO.DLS.Bookmark"/>
    /// in the document.
    /// </summary>
    public class BookmarkStart
      : ParagraphItem
#if !SILVERLIGHT && !WP
      ,ILeafWidget
#endif
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private string m_strName = "";
        private bool m_isCellGroupBkmk;
        /// <summary>
        /// Specifies whether the Bookmark is detached (For both cases either cloned or removed from collection) from the Bookmarks collection.
        /// </summary>
        internal bool m_isDetached = false;
        private int m_colFirst = -1;
        private int m_colLast = -1;
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
                return EntityType.BookmarkStart;
            }
        }
        /// <summary>
        /// Gets the bookmark name.
        /// </summary>
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
        /// <summary>
        /// Gets or sets the index of the column where bookmark starts.
        /// </summary>
        /// <value>The column first.</value>
        internal int ColumnFirst
        {
            get
            {
                return m_colFirst;
            }
            set
            {
                m_colFirst = value;
            }
        }
        /// <summary>
        /// Gets or sets the index of the column where bookmarks ends.
        /// </summary>
        /// <value>The column last.</value>
        internal int ColumnLast
        {
            get
            {
                return m_colLast;
            }
            set
            {
                m_colLast = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkStart"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal BookmarkStart(WordDocument doc)
            : this(doc, "")
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkStart"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="name">The name.</param>
        public BookmarkStart(IWordDocument doc, string name)
            : base((WordDocument)doc)
        {
            SetName(name);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the bookmark name.
        /// </summary>
        /// <param name="name">The name.</param>
        internal void SetName(string name)
        {
            m_strName = name.Replace('-', '_');

            //Can not truncate name for simple bookmarks (not formfields).
            //      if( m_strName.Length > 19 )
            //        m_strName = m_strName.Substring( 0, 20 );
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
                Document.Bookmarks.AttachBookmarkStart(this);
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
                BookmarkCollection bookmarks = Document.Bookmarks;
                Bookmark bkmk = bookmarks.FindByName(Name);

                if (bkmk != null)
                {
                    bkmk.SetStart(null);
                    bookmarks.Remove(bkmk);
                }
            }
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        internal override void CloneCommit()
        {
            if (m_isDetached)
            {
                Document.Bookmarks.AttachBookmarkStart(this);
                m_isDetached = false;
            }
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {           
            BookmarkStart bs = (BookmarkStart)base.CloneImpl();
            bs.m_isDetached = true;
            return bs;
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

            writer.WriteValue(PropertyNames.Type, ParagraphItemType.BookmarkStart);
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
            Document.Bookmarks.AttachBookmarkStart(this);

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
            m_layoutInfo.IsSkipBottomAlign = true;
            m_layoutInfo.IsClipped = (this.GetOwnerParagraph() as IWidget).LayoutInfo.IsClipped;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
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

        /// <summary>
        /// Imlementation of Draw method of IWidget interface .
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        void IWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget)
        {
            //Handled to preserve MS Word behavior, Bookmark name starts with underscore are considered as hidden bookmark.
            if (!Name.StartsWith("_"))
                Rendering.DocumentLayouter.Bookmarks.Add(new BookmarkPosition(Name, Rendering.DocumentLayouter.PageNumber, ltWidget.Bounds));
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
        #endregion
#endif
//#endif

    }
}
