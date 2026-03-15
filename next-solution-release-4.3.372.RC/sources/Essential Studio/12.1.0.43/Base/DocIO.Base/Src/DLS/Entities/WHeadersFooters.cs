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
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents page headers and footers
    /// </summary>
    public class WHeadersFooters : XDLSSerializableBase, IEnumerable
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private HeaderFooter m_evenHeader = null;
        private HeaderFooter m_oddFooter = null;
        private HeaderFooter m_oddHeader = null;
        private HeaderFooter m_evenFooter = null;
        private HeaderFooter m_firstPageHeader = null;
        private HeaderFooter m_firstPageFooter = null;
        private HFEnumerator m_enumer = null;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets default header.
        /// </summary>
        public HeaderFooter Header
        {
            get
            {
                return OddHeader;
            }
        }
        /// <summary>
        /// Gets default footer.
        /// </summary>
        public HeaderFooter Footer
        {
            get
            {
                return OddFooter;
            }
        }
        /// <summary>
        /// Gets even header.
        /// </summary>
        public HeaderFooter EvenHeader
        {
            get
            {
                return m_evenHeader;
            }
        }
        /// <summary>
        /// Gets odd header ( This is also the default header ).
        /// </summary>
        public HeaderFooter OddHeader
        {
            get
            {
                return m_oddHeader;
            }
        }
        /// <summary>
        /// Gets even footer
        /// </summary>
        public HeaderFooter EvenFooter
        {
            get
            {
                return m_evenFooter;
            }
        }
        /// <summary>
        /// Gets odd footer ( This is also the default footer ).
        /// </summary>
        public HeaderFooter OddFooter
        {
            get
            {
                return m_oddFooter;
            }
        }
        /// <summary>
        /// Gets first page header.
        /// </summary>
        public HeaderFooter FirstPageHeader
        {
            get
            {
                return m_firstPageHeader;
            }
        }
        /// <summary>
        /// Gets first page footer.
        /// </summary>
        public HeaderFooter FirstPageFooter
        {
            get
            {
                return m_firstPageFooter;
            }
        }
        /// <summary>
        /// Detects whether all headers/footers are empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (m_evenHeader.ChildEntities.Count == 0 && m_evenFooter.ChildEntities.Count == 0 &&
                         m_oddFooter.ChildEntities.Count == 0 && m_oddHeader.ChildEntities.Count == 0 &&
                         m_firstPageFooter.ChildEntities.Count == 0 && m_firstPageHeader.ChildEntities.Count == 0);
            }
        }
        /// <summary>
        /// Gets TextBody at specified index.
        /// </summary>
        public HeaderFooter this[int index]
        {
            get
            {
                if (index < 0 || index > 5)
                    throw new ArgumentOutOfRangeException("index", "index cann't be less 0 or greater 5");

                return this[(HeaderFooterType)index];
            }
        }
        /// <summary>
        /// Gets TextBody by specified HeaderFooter type.
        /// </summary>
        public HeaderFooter this[HeaderFooterType hfType]
        {
            get
            {
                switch (hfType)
                {
                    case HeaderFooterType.EvenHeader:
                        return EvenHeader;
                    case HeaderFooterType.OddHeader:
                        return OddHeader;
                    case HeaderFooterType.EvenFooter:
                        return EvenFooter;
                    case HeaderFooterType.OddFooter:
                        return OddFooter;
                    case HeaderFooterType.FirstPageHeader:
                        return FirstPageHeader;
                    case HeaderFooterType.FirstPageFooter:
                        return FirstPageFooter;
                }

                throw new ArgumentException("Invalid header/footer type", "hfType");
            }
            internal set
            {
                switch (hfType)
                {
                    case HeaderFooterType.EvenHeader:
                        m_evenHeader = value;
                        break;
                    case HeaderFooterType.OddHeader:
                        m_oddHeader = value;
                        break;
                    case HeaderFooterType.EvenFooter:
                        m_evenFooter = value;
                        break;
                    case HeaderFooterType.OddFooter:
                        m_oddFooter = value;
                        break;
                    case HeaderFooterType.FirstPageHeader:
                        m_firstPageHeader = value;
                        break;
                    case HeaderFooterType.FirstPageFooter:
                        m_firstPageFooter = value;
                        break;
                    default:
                        throw new ArgumentException("Invalid header/footer type", "hfType");
                }

            }
        }
        /// <summary>
        /// If set to True if this header/footer is linked to the header/footer in the previous section.
        /// </summary>
        public bool LinkToPrevious
        {
            get
            {
                return GetLinkToPrevious();
            }
            set
            {
                UpdateLinkToPrevious(value);
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Creates HeadersFooters object for specified document.
        /// </summary>
        internal WHeadersFooters(WSection sec)
            : base(sec.Document, sec)
        {
            m_evenHeader = new HeaderFooter(sec, HeaderFooterType.EvenHeader);
            m_oddHeader = new HeaderFooter(sec, HeaderFooterType.OddHeader);
            m_evenFooter = new HeaderFooter(sec, HeaderFooterType.EvenFooter);
            m_oddFooter = new HeaderFooter(sec, HeaderFooterType.OddFooter);
            m_firstPageFooter = new HeaderFooter(sec, HeaderFooterType.FirstPageFooter);
            m_firstPageHeader = new HeaderFooter(sec, HeaderFooterType.FirstPageHeader);
        }
        #endregion

        #region XML serialization overrides
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.EvenHeaderTag, EvenHeader);
            XDLSHolder.AddElement(XDLSConstants.OddHeaderTag, OddHeader);
            XDLSHolder.AddElement(XDLSConstants.EvenFooterTag, EvenFooter);
            XDLSHolder.AddElement(XDLSConstants.OddFooterTag, OddFooter);
            XDLSHolder.AddElement(XDLSConstants.FirstPageHeaderTag, FirstPageHeader);
            XDLSHolder.AddElement(XDLSConstants.FirstPageFooterTag, FirstPageFooter);
        }
//#endif
        #endregion

        #region Implementation
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal WHeadersFooters Clone()
        {
            return (WHeadersFooters)CloneImpl();
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WHeadersFooters hfs = (WHeadersFooters)base.CloneImpl();
            hfs.m_evenHeader = (HeaderFooter)m_evenHeader.Clone();
            hfs.m_oddHeader = (HeaderFooter)m_oddHeader.Clone();
            hfs.m_evenFooter = (HeaderFooter)m_evenFooter.Clone();
            hfs.m_oddFooter = (HeaderFooter)m_oddFooter.Clone();
            hfs.m_firstPageFooter = (HeaderFooter)m_firstPageFooter.Clone();
            hfs.m_firstPageHeader = (HeaderFooter)m_firstPageHeader.Clone();

            return hfs;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            HeaderFooter hf = null;
            for (int i = 0; i < 6; i++)
            {
                hf = this[i];
                hf.Close();
                hf = null;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            //if( m_enumer == null )
            //{
            //  m_enumer = new HFEnumerator( this );
            //}
            //return m_enumer;
            return new HFEnumerator(this);
        }
        #endregion

        #region Internal declarations
        /// <summary>
        /// 
        /// </summary>
        internal class HFEnumerator : IEnumerator
        {
            #region IEnumerator Members
            private int m_index = -1;
            private WHeadersFooters m_hfs;
            /// <summary>
            /// Initializes a new instance of the <see cref="HFEnumerator"/> class.
            /// </summary>
            /// <param name="hfs">The HFS.</param>
            internal HFEnumerator(WHeadersFooters hfs)
            {
                m_hfs = hfs;
            }
            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <value></value>
            /// <returns>The current element in the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element. </exception>
            public object Current
            {
                get
                {
                    return (m_index < 0) ? null : m_hfs[m_index];
                }
            }
            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext()
            {
                if (m_index < 5)
                {
                    m_index++;
                    return true;
                }

                return false;
            }
            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset()
            {
                m_index = -1;
            }
            #endregion
        }
        #endregion

        #region Implementation / link to previous
        /// <summary>
        /// Gets the link to previous.
        /// </summary>
        /// <returns></returns>
        private bool GetLinkToPrevious()
        {
            WSection owner = this.OwnerBase as WSection;
            if (owner == null)
                return false;

            int sectionIndex = owner.GetIndexInOwnerCollection();
            if (sectionIndex > 0)
            {
                bool linkToPrevious = true;
                for (int i = 0; i < 6; i++)
                {
                    if (this[i].Items.Count > 0)
                    {
                        linkToPrevious = false;
                        break;
                    }
                }
                return linkToPrevious;
            }

            return false;
        }
        /// <summary>
        /// Updates the link to previous.
        /// </summary>
        /// <param name="linkToPrevious">if it links to previous header/footer, set to <c>true</c>.</param>
        private void UpdateLinkToPrevious(bool linkToPrevious)
        {
            WSection owner = this.OwnerBase as WSection;
            if (owner == null)
                return;

            // When set property to non attached to document section.
            int sectionIndex = owner.GetIndexInOwnerCollection();
            if (sectionIndex > 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    this[i].LinkToPrevious = linkToPrevious;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Represents page header or footer
    /// </summary>
    public class HeaderFooter : WTextBody
    {
        #region Field
        private HeaderFooterType m_type;
        private bool m_writeWatermark;
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
                return EntityType.HeaderFooter;
            }
        }
        /// <summary>
        /// Gets or sets the type of current header/footer.
        /// </summary>
        /// <value>The type.</value>
        internal HeaderFooterType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether need write watermark.
        /// </summary>
        /// <value><c>true</c> if write watermark; otherwise, <c>false</c>.</value>
        internal bool WriteWatermark
        {
            get
            {
                WSection section = this.OwnerBase as WSection;
                if (m_writeWatermark)
                {
                    return true;
                }

                return false;
            }
            set
            {
                m_writeWatermark = value;
            }
        }
        /// <summary>
        /// If set to True if this header/footer is linked to the header/footer in the previous section.
        /// </summary>
        public bool LinkToPrevious
        {
            get
            {
                return GetLinkToPrevious();
            }
            set
            {
                UpdateLinkToPrevious(value);
            }
        }
        /// <summary>
        /// Checks the m_writeWatermark variable.
        /// </summary>
        /// <returns></returns>
        internal bool CheckWriteWatermark()
        {
            return m_writeWatermark;
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooter"/> class.
        /// </summary>
        /// <param name="sec">The sec.</param>
        internal HeaderFooter(WSection sec, HeaderFooterType type)
            : base(sec)
        {
            m_type = type;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the link to previous.
        /// </summary>
        /// <returns></returns>
        private bool GetLinkToPrevious()
        {
            WSection owner = this.OwnerBase as WSection;
            if (owner == null)
                return false;

            int sectionIndex = owner.GetIndexInOwnerCollection();

            if (sectionIndex > 0)
                return (this.Items.Count == 0);

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void UpdateLinkToPrevious(bool linkToPrevious)
        {
            if (linkToPrevious)
            {
                this.ChildEntities.Clear();
                (this.OwnerBase as WSection).HeadersFooters[m_type] = new HeaderFooter(this.OwnerBase as WSection, m_type);
            }
            else
            {
                WSection sourceSection = FindSourceSection();
                if (sourceSection != null)
                {
                    foreach (HeaderFooter headerFooter in sourceSection.HeadersFooters)
                    {
                        if (this.m_type == headerFooter.m_type && CheckShapes(headerFooter))
                        {
                            headerFooter.m_bodyItems.CloneTo(this.m_bodyItems);
                        }
                    }
                }
                //Updates the current header footer with empty paragraph similar to MS Word behavior.
                if (this.ChildEntities.Count == 0)
                    this.AddParagraph();
            }
        }
        /// <summary>
        /// Finds the source header/footer.
        /// </summary>
        /// <returns></returns>
        private WSection FindSourceSection()
        {
            WSection section = (this.OwnerBase as WSection).PreviousSibling as WSection;

            while (section != null)
            {
                WHeadersFooters hf = section.HeadersFooters;
                if (!hf.LinkToPrevious)
                {
                    return section;
                }
                section = section.PreviousSibling as WSection;
            }
            return section;
        }
        /// <summary>
        /// Checks the shapes in header/footer.
        /// </summary>
        /// <param name="hf">The hf.</param>
        private bool CheckShapes(HeaderFooter hf)
        {
            foreach (WParagraph para in hf.Paragraphs)
            {
                foreach (ParagraphItem item in para.Items)
                {
                    if (item is WPicture || item is WTextBox)
                    {
                        item.Cloned = true;
                    }
                    else if (item is ShapeObject)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion
    }
}