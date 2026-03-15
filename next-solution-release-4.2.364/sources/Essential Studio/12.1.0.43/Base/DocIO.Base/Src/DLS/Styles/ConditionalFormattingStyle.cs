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
    /// Represents a conditional style of table. 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ConditionalFormattingStyle : Style
    {
        #region Fields
        private WParagraphFormat m_paragraphFormat;
        private TableStyleCellProperties m_cellProperties;
        private TableStyleRowProperties m_rowProperties;
        private TableStyleTableProperties m_tableProperties;
        private ConditionalFormattingCode m_conditionalFormattingType;
        #endregion

        #region Properties
        /// <summary>
        /// Gets paragraph format.
        /// </summary>
        /// <value></value>
        internal WParagraphFormat ParagraphFormat
        {
            get
            {
                return m_paragraphFormat;
            }
        }
        /// <summary>
        /// Gets cell properties.
        /// </summary>
        /// <value></value>
        internal TableStyleCellProperties CellProperties
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
        internal TableStyleRowProperties RowProperties
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
        internal TableStyleTableProperties TableProperties
        {
            get
            {
                return m_tableProperties;
            }
        }
        /// <summary>
        /// Gets conditional formatting code.
        /// </summary>
        /// <value></value>
        internal ConditionalFormattingCode ConditionalFormattingType
        {
            get
            {
                return m_conditionalFormattingType;
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
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ConditionalFormattingStyle class.
        /// </summary>
        /// <param name="conditionCode">The conditionCode.</param>
        /// <param name="doc">The doc.</param>
        internal ConditionalFormattingStyle(ConditionalFormattingCode conditionCode, IWordDocument doc)
            : base((WordDocument)doc)
        {
            m_conditionalFormattingType = conditionCode;
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
        /// Clones itself
        /// </summary>
        /// <returns></returns>
        public override IStyle Clone()
        {
            return (ConditionalFormattingStyle)CloneImpl();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            ConditionalFormattingStyle cs = (ConditionalFormattingStyle)base.CloneImpl();
            cs.m_paragraphFormat = new WParagraphFormat(Document);
            cs.m_paragraphFormat.ImportContainer(ParagraphFormat);
            cs.m_paragraphFormat.SetOwner(cs);
            cs.m_cellProperties = new TableStyleCellProperties(Document);
            cs.m_cellProperties.ImportContainer(CellProperties);
            cs.m_cellProperties.SetOwner(this);
            cs.m_rowProperties = new TableStyleRowProperties(Document);
            cs.m_rowProperties.ImportContainer(RowProperties);
            cs.m_rowProperties.SetOwner(this);
            cs.m_tableProperties = new TableStyleTableProperties(Document);
            cs.m_tableProperties.ImportContainer(TableProperties);
            cs.m_tableProperties.SetOwner(this);
            return cs;
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
