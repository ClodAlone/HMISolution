#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.ExcelToPdfConverter
{
    /// <summary>
    /// 
    /// </summary>
    public class HeaderFooterOption
    {
        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private bool m_showHeader;
        /// <summary>
        /// 
        /// </summary>
        private bool m_showFooter;
        #endregion

        #region Constructor
        public HeaderFooterOption()
        {
            m_showFooter = true;
            m_showHeader = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [show header].
        /// </summary>
        /// <value><c>true</c> if [show header]; otherwise, <c>false</c>.</value>
        public bool ShowHeader
        {
            get
            {
                return m_showHeader;
            }
            set
            {
                m_showHeader = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show footer].
        /// </summary>
        /// <value><c>true</c> if [show footer]; otherwise, <c>false</c>.</value>
        public bool ShowFooter
        {
            get
            {
                return m_showFooter;
            }
            set
            {
                m_showFooter = value;
            }
        }
        #endregion
    }
}
