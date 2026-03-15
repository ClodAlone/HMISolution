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
using System.Collections.Generic;
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a style of table. 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class WTableStyle
      : Style, IWTableStyle
    {

        #region Fields
        private WParagraphFormat m_paragraphFormat;
        private WListFormat m_listFormat;
        private TableStyleCellProperties m_cellProperties;
        private TableStyleRowProperties m_rowProperties;
        private TableStyleTableProperties m_tableProperties;
        private Dictionary<ConditionalFormattingCode, ConditionalFormattingStyle> m_conditionalFormattingStyles = new Dictionary<ConditionalFormattingCode,ConditionalFormattingStyle>();
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
        /// Gets the list format.
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
        /// Gets cell properties.
        /// </summary>
        /// <value></value>
        public TableStyleCellProperties CellProperties
        {
            get
            {
                return m_cellProperties;
            }
        }
        /// <summary>
        /// Gets row properties.
        /// </summary>
        /// <value></value>
        public TableStyleRowProperties RowProperties
        {
            get
            {
                return m_rowProperties;
            }
        }
        /// <summary>
        /// Gets table properties.
        /// </summary>
        /// <value></value>
        public TableStyleTableProperties TableProperties
        {
            get
            {
                return m_tableProperties;
            }
        }
        /// <summary>
        /// Gets base style of the current style.
        /// </summary>
        new internal WTableStyle BaseStyle
        {
            get
            {
                return base.BaseStyle as WTableStyle;
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
                return StyleType.TableStyle;
            }
        }
        /// <summary>
        /// Gets conditional formatting styles collection of the table style.
        /// </summary>
        internal Dictionary<ConditionalFormattingCode, ConditionalFormattingStyle> ConditionalFormattingStyles
        {
            get
            {
                return m_conditionalFormattingStyles;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the WTableStyle class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal WTableStyle(IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_paragraphFormat = new WParagraphFormat(Document);
            m_paragraphFormat.SetOwner(this);
            m_cellProperties = new TableStyleCellProperties(Document);
            m_cellProperties.SetOwner(this);
            m_rowProperties = new TableStyleRowProperties(Document);
            m_rowProperties.SetOwner(this);
            m_tableProperties = new TableStyleTableProperties(Document);
            m_tableProperties.SetOwner(this);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Create conditional formatting style for current style.
        /// </summary>
        /// <param name="conditionCode"></param>
        /// <returns></returns>
        internal ConditionalFormattingStyle ConditionalFormat(ConditionalFormattingCode conditionCode)
        {
            ConditionalFormattingStyle conditionalStyle = new ConditionalFormattingStyle(conditionCode, Document);
            m_conditionalFormattingStyles.Add(conditionCode, conditionalStyle);
            return conditionalStyle;
        }
        /// <summary>
        /// Apply base style for current style.
        /// </summary>
        /// <param name="styleName"></param>
        public override void ApplyBaseStyle(string styleName)
        {
            base.ApplyBaseStyle(styleName);

            if (BaseStyle != null)
            {
                m_paragraphFormat.ApplyBase(BaseStyle.ParagraphFormat);
                m_cellProperties.ApplyBase(BaseStyle.CellProperties);
                m_rowProperties.ApplyBase(BaseStyle.RowProperties);
                m_tableProperties.ApplyBase(BaseStyle.TableProperties);
            }
        }
        /// <summary>
        /// Clones itself
        /// </summary>
        /// <returns></returns>
        public override IStyle Clone()
        {
            return (WTableStyle)CloneImpl();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WTableStyle ts = (WTableStyle)base.CloneImpl();

            ts.m_paragraphFormat = new WParagraphFormat(Document);
            ts.m_paragraphFormat.ImportContainer(ParagraphFormat);
            ts.m_paragraphFormat.CopyFormat(ParagraphFormat);
            ts.m_paragraphFormat.SetOwner(ts);
            ts.m_cellProperties = new TableStyleCellProperties(Document);
            ts.m_cellProperties.ImportContainer(CellProperties);
            ts.m_cellProperties.SetOwner(this);
            ts.m_rowProperties = new TableStyleRowProperties(Document);
            ts.m_rowProperties.ImportContainer(RowProperties);
            ts.m_rowProperties.SetOwner(this);
            ts.m_tableProperties = new TableStyleTableProperties(Document);
            ts.m_tableProperties.ImportContainer(TableProperties);
            ts.m_tableProperties.SetOwner(this);
            return ts;
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
            if (m_cellProperties != null)
            {
                m_cellProperties.Close();
                m_cellProperties = null;
            }
            if (m_rowProperties != null)
            {
                m_rowProperties.Close();
                m_rowProperties = null;
            }
            if (m_tableProperties != null)
            {
                m_tableProperties.Close();
                m_tableProperties = null;
            }
        }
        #endregion
    }
}
