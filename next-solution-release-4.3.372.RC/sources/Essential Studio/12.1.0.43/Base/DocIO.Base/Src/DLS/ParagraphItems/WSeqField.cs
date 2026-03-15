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
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WSeqField.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WSeqField : WField
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private CaptionNumberingFormat m_numberFormat = (CaptionNumberingFormat)(-1);
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
                return EntityType.SeqField;
            }
        }
        /// <summary>
        /// Gets the formatting string
        /// </summary>
        public string FormattingString
        {
            get
            {
                return m_formattingString;
            }
        }
        /// <summary>
        /// Gets / sets the type of caption numbering
        /// </summary>
        public CaptionNumberingFormat NumberFormat
        {
            get
            {
                return m_numberFormat;
            }
            set
            {
                m_numberFormat = value;
                m_formattingString = ConvertSwitchesToString();
            }
        }
        /// <summary>
        /// Gets / sets caption name
        /// </summary>
        public string CaptionName
        {
            get
            {
                return m_fieldValue;
            }
            set
            {
                m_fieldValue = value;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WSeqField"/> class.
        /// </summary>
        /// <param name="doc"></param>
        public WSeqField(IWordDocument doc)
            : base(doc)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="WSeqField"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        protected internal WSeqField(WField field)
            : base(field.Document)
        { }
        #endregion

        #region Class public methods
        /// <summary>
        /// Removes the Switch String befor apply new switch
        /// </summary>
        /// <returns></returns>
        private void ClearSwitchString()
        {
            m_formattingString = m_formattingString.Replace(@"\* ARABIC", string.Empty);
            m_formattingString = m_formattingString.Replace(@"\* ALPHABETIC", string.Empty);
            m_formattingString = m_formattingString.Replace(@"\*ROMAN", string.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected internal override string ConvertSwitchesToString()
        {
            string switches =string.Empty ;

            switch (m_numberFormat)
            {
                case CaptionNumberingFormat.Number:
                    ClearSwitchString();
                    switches += " \\* ARABIC";
                    break;
                case CaptionNumberingFormat.Alphabetic:
                    ClearSwitchString();
                    switches += " \\* ALPHABETIC";
                    break;
                case CaptionNumberingFormat.Roman:
                    ClearSwitchString();
                    switches += " \\* ROMAN";
                    break;
            }
            switches = base.ConvertSwitchesToString() + switches;
            return switches;
        }
        #endregion
    }
}