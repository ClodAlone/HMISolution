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
    /// Represents a style of paragraph. 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WParagraphStyle
      : Style
      , IWParagraphStyle
    {
        #region Fields
        protected WParagraphFormat m_prFormat;
        protected WListFormat m_listFormat;
        private int m_listIndex = -1;
        private int m_listLevel = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Gets formatting of paragraph.
        /// </summary>
        /// <value></value>
        public WParagraphFormat ParagraphFormat
        {
            get
            {
                return m_prFormat;
            }
        }
        /// <summary>
        /// Gets a base style of paragraph.
        /// </summary>
        new public WParagraphStyle BaseStyle
        {
            get
            {
                return base.BaseStyle as WParagraphStyle;
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
                return StyleType.ParagraphStyle;
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
        public WParagraphStyle(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_prFormat = new WParagraphFormat(Document);
            m_prFormat.SetOwner(this);

            //Applies normal paragraph style as default base style.
            if ((doc as WordDocument).CreateBaseStyle)
            {
                (doc as WordDocument).CreateBaseStyle = false;
                ApplyBaseStyle(BuiltinStyle.Normal);
                //Handled to avoid repeated looping while creating base paragraph style - Normal (Recursive call).
                (doc as WordDocument).CreateBaseStyle = true;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Apply base style for current style.
        /// </summary>
        /// <param name="styleName"></param>
        public override void ApplyBaseStyle(string styleName)
        {
            base.ApplyBaseStyle(styleName);

            if (BaseStyle != null)
                m_prFormat.ApplyBase(BaseStyle.ParagraphFormat);
        }
        /// <summary>
        /// Clones itself
        /// </summary>
        /// <returns></returns>
        public override IStyle Clone()
        {
            return (WParagraphStyle)CloneImpl();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WParagraphStyle ps = (WParagraphStyle)base.CloneImpl();

            ps.m_prFormat = new WParagraphFormat(Document);
            ps.m_prFormat.ImportContainer(ParagraphFormat);
            ps.m_prFormat.CopyFormat(ParagraphFormat);
            ps.m_prFormat.SetOwner(ps);

            ps.m_listFormat = new WListFormat(Document, this);
            ps.m_listFormat.ImportContainer(ListFormat);
            ps.m_listFormat.SetOwner(ps);

            if (BaseStyle != null)
            {
                ps.ApplyBaseStyle(BaseStyle.Name);
            }
            return ps;
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

            if (m_prFormat != null)
            {
                m_prFormat.Close();
                m_prFormat = null;
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
            XDLSHolder.AddElement(XDLSConstants.ParagraphFormatTag, m_prFormat);
        }

        #endregion
//#endif
    }
}