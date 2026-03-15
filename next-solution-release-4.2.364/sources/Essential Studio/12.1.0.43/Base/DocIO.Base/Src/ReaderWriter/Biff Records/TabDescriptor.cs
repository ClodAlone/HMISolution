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
using Syncfusion.DocIO.DLS;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for TabDescriptor.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class TabDescriptor
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        internal const int DEF_TAB_LENGTH = 1;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private TabJustification m_jc;
        /// <summary>
        /// 
        /// </summary>
        private TabLeader m_tlc;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the justification.
        /// </summary>
        /// <value>The justification.</value>
        internal TabJustification Justification
        {
            get
            {
                return m_jc;
            }
            set
            {
                if (value != m_jc)
                {
                    m_jc = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the tab leader.
        /// </summary>
        /// <value>The tab leader.</value>
        internal TabLeader TabLeader
        {
            get
            {
                return m_tlc;
            }
            set
            {
                if (value != m_tlc)
                {
                    m_tlc = value;
                }
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        internal TabDescriptor(byte options)
        {
            m_jc = (TabJustification)((byte)(options & 7));
            m_tlc = (TabLeader)((byte)((options & 0x38) >> 3));
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TabDescriptor"/> class.
        /// </summary>
        /// <param name="justification">The justification.</param>
        /// <param name="leader">The leader.</param>
        internal TabDescriptor(TabJustification justification, TabLeader leader)
        {
            m_jc = justification;
            m_tlc = leader;
        }
        #endregion

        #region Class internal methods
        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        internal byte Save()
        {
            int num1 = (((byte)m_tlc) << 3) | (byte)m_jc;
            return (byte)num1;
        }
        #endregion
    }
}                                                          
