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
    /// Represents a style of Numbering
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WNumberingStyle
      : Style
    {
        #region Fields
        private WParagraphFormat m_paragraphFormat;
        protected WListFormat m_listFormat;
        private int m_listIndex = -1;
        private int m_listLevel = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Gets paragraph format.
        /// </summary>
        /// <value></value>
        public WParagraphFormat ParagraphFormat
        {
            get
            {
                return m_paragraphFormat;
            }
        }
        /// <summary>
        /// Gets a base style of paragraph.
        /// </summary>
        new public WNumberingStyle BaseStyle
        {
            get
            {
                return base.BaseStyle as WNumberingStyle;
            }
        }
        /// <summary>
        /// Gets the type of the style.
        /// </summary>
        /// <value>The type of the style.</value>
        public override StyleType StyleType
        {
            get
            {
                return StyleType.NumberingStyle;
            }
        }
        /// <summary>
        /// Gets format of the list for the paragraph.
        /// </summary>
        public WListFormat ListFormat
        {
            get
            {
                if (m_listFormat == null)
                {
                    m_listFormat = new WListFormat(this.Document, this);
                }
                return m_listFormat;
            }
        }
        /// <summary>
        /// Gets or sets the index of the list.
        /// </summary>
        /// <value>The index of the list.</value>
        internal int ListIndex
        {
            get
            {
                return m_listIndex;
            }
            set
            {
                m_listIndex = value;
            }
        }
        /// <summary>
        /// Gets or sets the list level.
        /// </summary>
        /// <value>The list level.</value>
        internal int ListLevel
        {
            get
            {
                return m_listLevel;
            }
            set
            {
                m_listLevel = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WParagraphStyle"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal WNumberingStyle(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_paragraphFormat = new WParagraphFormat(Document);
            m_paragraphFormat.SetOwner(this);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Clones itself
        /// </summary>
        /// <returns></returns>
        public override IStyle Clone()
        {
            return (WNumberingStyle)CloneImpl();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WNumberingStyle ns = (WNumberingStyle)base.CloneImpl();
            ns.m_paragraphFormat = new WParagraphFormat(Document);
            ns.m_paragraphFormat.ImportContainer(ParagraphFormat);
            ns.m_paragraphFormat.SetOwner(ns);
            ns.m_listFormat = new WListFormat(Document, this);
            ns.m_listFormat.ImportContainer(ListFormat);
            ns.m_listFormat.SetOwner(ns);
            return ns;
        }

        /// <summary>
        /// Clones the list style.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal void CloneListRelationsTo(WordDocument doc)
        {
            if (ListFormat.ListType != ListType.NoList)
            {
                ListStyle lstStyle = ListFormat.CurrentListStyle;

                if (lstStyle != null && doc.ListStyles.FindByName(lstStyle.Name) == null)
                {
                    doc.ListStyles.Add((ListStyle)lstStyle.Clone());
                }
                if (lstStyle != null
                    && ListFormat.CurrentListLevel != null)
                      ListFormat.CurrentListLevel.ParaStyleName = this.Name.Replace(" ", string.Empty);
           }

            if (ListFormat.LFOStyleName == null)
                return;

            if (doc.ListOverrides.FindByName(ListFormat.LFOStyleName) == null)
            {
                ListOverrideStyle lfoStyle = this.Document.ListOverrides.FindByName(ListFormat.LFOStyleName);
                if (lfoStyle != null) doc.ListOverrides.Add((ListOverrideStyle)lfoStyle.Clone());
            }
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            base.Close();

            if (m_paragraphFormat != null)
            {
                m_paragraphFormat.Close();
                m_paragraphFormat = null;
            }
        }
        #endregion
//#if !SILVERLIGHT
        #region Implementation / xml
        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            base.InitXDLSHolder();
            XDLSHolder.AddElement(XDLSConstants.ParagraphFormatTag, m_paragraphFormat);
        }

        #endregion
//#endif
    }
}